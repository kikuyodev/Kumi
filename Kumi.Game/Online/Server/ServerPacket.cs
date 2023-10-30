using Newtonsoft.Json;

namespace Kumi.Game.Online.Server;

/// <summary>
/// A server packet with consistent data.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ServerPacket<T> : ServerPacket
    where T : class
{
    [JsonProperty("d")]
    public new T Data { get; set; }
    
    public ServerPacket()
    {
        
    }

    public ServerPacket(ServerPacket<T> parent)
        : base(parent)
    {
    }
}

/// <summary>
/// A server packet, which contains an opcode, but no data.
/// </summary>
public class ServerPacket
{
    [JsonProperty("op")]
    public ServerPacketOpCode OpCode { get; set; }
    
    public string RawData { get; set; } = string.Empty;

    public ServerPacket()
    {
        
    }

    public ServerPacket(ServerPacket parent)
    {
        OpCode = parent.OpCode;
    }
}
