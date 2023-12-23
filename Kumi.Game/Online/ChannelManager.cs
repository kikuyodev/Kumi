using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Requests;
using Kumi.Game.Online.Channels;
using Kumi.Game.Online.Server;
using Kumi.Game.Online.Server.Packets.Dispatch;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Threading;

namespace Kumi.Game.Online;

[Cached]
public partial class ChannelManager : Component
{
    private readonly BindableList<Channel> channels = new BindableList<Channel>();
    public IBindableList<Channel> Channels => channels;

    private readonly BindableList<Channel> subscribedChannels = new BindableList<Channel>();
    public IBindableList<Channel> SubscribedChannels => subscribedChannels;

    public Bindable<Channel> CurrentChannel { get; } = new Bindable<Channel>();

    [Resolved]
    private IAPIConnectionProvider API { get; set; } = null!;

    private IServerConnector connector = null!;

    private ScheduledDelegate? connectorLoadDelegate;

    protected override void LoadComplete()
    {
        Scheduler.Add(connectorLoadDelegate = new ScheduledDelegate(() =>
        {
            var serverConnector = API.GetServerConnector();
            if (serverConnector == null)
                return;

            if (!(serverConnector.CurrentConnection?.IsConnected ?? false))
                return;

            connectorLoadDelegate?.Cancel();
            connector = serverConnector;
            onConnectorLoaded();
        }, 0, 200));
    }

    public void JoinChannel(Channel channel)
    {
        var req = new JoinChannelRequest { Channel = channel.APIChannel };
        req.Success += () =>
        {
            if (!subscribedChannels.Contains(channel))
                subscribedChannels.Add(channel);

            CurrentChannel.Value = channel;
        };

        API.PerformAsync(req);
    }

    private void onConnectorLoaded()
    {
        connector.RegisterDispatchHandler<ChatChannelAddEvent>("CHAT_CHANNEL_CREATE", e => onChannelAdd(e.Data));
        connector.RegisterDispatchHandler<ChatMessageEvent>("CHAT_MESSAGE_CREATE", e => onMessage(e.Data));

        var channelsListRequest = new ChannelListRequest();
        channelsListRequest.Success += () =>
        {
            var list = channelsListRequest.Response.GetChannels();

            foreach (var channel in list)
                channels.Add(new Channel(channel));
        };

        API.Perform(channelsListRequest);
    }

    private void onChannelAdd(ChatChannelAddEvent.ChatChannelAddEventData channel)
    {
        if (channels.Any(c => c.APIChannel.Id == channel.Channel.Id))
            return;

        channels.Add(new Channel(channel.Channel));
    }

    private void onMessage(ChatMessageEvent.ChatMessageEventData message)
    {
        var channel = channels.FirstOrDefault(c => c.APIChannel.Id == message.Channel.Id);
        channel?.AddMessage(message.Message);
    }
}
