using Newtonsoft.Json;

namespace Kumi.Game.Online.Server.Packets;

public class DispatchPacket<T> : Packet<T> 
    where T : class
{
    public override OpCode OpCode => OpCode.Dispatch;
    
    /// <summary>
    /// The type of dispatch this packet is.
    /// </summary>
    [JsonProperty("t")]
    public new virtual string? DispatchType { get; set; } = null!;
}
