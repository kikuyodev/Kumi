using Newtonsoft.Json;

namespace Kumi.Game.Online.Server.Packets.Dispatch;

public class NotificationDispatchPacket : DispatchPacket<NotificationDispatchPacket.NotificationDispatchData>
{
    public override string DispatchType => "NOTIFICATION";

    public class NotificationDispatchData
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
