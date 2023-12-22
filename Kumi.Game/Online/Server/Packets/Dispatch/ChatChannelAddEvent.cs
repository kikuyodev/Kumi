using Kumi.Game.Online.API.Chat;
using Newtonsoft.Json;

namespace Kumi.Game.Online.Server.Packets.Dispatch;

public class ChatChannelAddEvent : DispatchPacket<ChatChannelAddEvent.ChatChannelAddEventData>
{
    public override string DispatchType => "CHAT_CHANNEL_CREATE";

    public class ChatChannelAddEventData
    {
        [JsonProperty("channel")]
        public APIChatChannel Channel { get; set; }
    }
}
