using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Online.API.Chat;
using Newtonsoft.Json;

namespace Kumi.Game.Online.Server.Packets.Dispatch;

public class ChatMessageEvent : DispatchPacket<ChatMessageEvent.ChatMessageEventData>
{
    public override string DispatchType => "CHAT_MESSAGE_CREATE";
    
    public class ChatMessageEventData
    {
        [JsonProperty("message")]
        public APIChatMessage Message { get; set; }
        
        [JsonIgnore]
        public string Content => Message.Content;
        
        [JsonIgnore]
        public APIChatChannel Channel => Message.Channel;
        
        [JsonIgnore]
        public APIAccount Account => Message.Account;
    }
}
