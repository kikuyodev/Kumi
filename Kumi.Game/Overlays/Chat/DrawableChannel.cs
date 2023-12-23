using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Chat;
using Kumi.Game.Online.Channels;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Overlays.Chat;

public partial class DrawableChannel : CompositeDrawable
{
    private readonly Channel boundChannel;

    public DrawableChannel(Channel boundChannel)
    {
        this.boundChannel = boundChannel;

        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding { Right = 12 };
    }

    private FillFlowContainer<MessageGroup> messages = null!;
    private KumiTextBox textBox = null!;

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new[]
            {
                new Dimension(),
                new Dimension(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new ChannelScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = messages = new FillFlowContainer<MessageGroup>
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 2),
                            Padding = new MarginPadding { Vertical = 8 },
                        }
                    }
                },
                new Drawable[]
                {
                    textBox = new ChannelTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 32,
                        PlaceholderText = $"Message #{boundChannel.APIChannel.Name}",
                        Padding = new MarginPadding { Bottom = 8 },
                        ReleaseFocusOnCommit = false,
                        LengthLimit = 255
                    }
                }
            }
        };

        textBox.OnCommit += sendMessage;
        boundChannel.OnNewMessage += addMessage;
    }

    private void sendMessage(TextBox sender, bool _)
    {
        if (string.IsNullOrWhiteSpace(sender.Text))
            return;

        // TODO: remove this and replace with an API call.
        addMessage(new APIChatMessage { Content = sender.Text });

        sender.Text = string.Empty;
    }

    private void addMessage(APIChatMessage message)
    {
        // check and see if the last message was sent by the same user
        var lastMessage = messages.Children.LastOrDefault();

        if (lastMessage?.Account.Id == api.LocalAccount.Value.Id)
        {
            // if so, add the message to the last message
            lastMessage.AddMessage(message.Content);
        }
        else
        {
            // otherwise, create a new message group
            var group = new MessageGroup(api.LocalAccount.Value);
            group.AddMessage(message.Content);
            messages.Add(group);
        }
    }
}
