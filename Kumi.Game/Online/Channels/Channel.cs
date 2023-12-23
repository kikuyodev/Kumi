using Kumi.Game.Online.API.Chat;
using osu.Framework.Bindables;

namespace Kumi.Game.Online.Channels;

public class Channel
{
    public Action<APIChatMessage>? OnNewMessage;

    public readonly APIChatChannel APIChannel;

    private readonly BindableList<APIChatMessage> messages = new BindableList<APIChatMessage>();
    public IBindableList<APIChatMessage> Messages => messages;

    public Channel(APIChatChannel apiChannel)
    {
        APIChannel = apiChannel;
    }

    public void AddMessage(APIChatMessage message)
    {
        messages.Add(message);
        OnNewMessage?.Invoke(message);
    }
}
