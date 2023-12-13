using Newtonsoft.Json;

namespace Kumi.Game.Online.Server;

/// <summary>
/// A server packet with consistent data.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Packet<T> : Packet
    where T : class
{
    [JsonProperty("d")]
    public T Data { get; set; } = null!;

    public Packet()
    {
    }

    public Packet(Packet<T> parent)
        : base(parent)
    {
        Data = parent.Data;
    }
}

/// <summary>
/// A server packet, which contains an opcode, but no data.
/// </summary>
public class Packet
{
    [JsonProperty("op")]
    public virtual OpCode OpCode { get; set; }

    [JsonIgnore]
    public string RawData { get; set; } = string.Empty;
    
    [JsonProperty("t")]
    public virtual string? DispatchType { get; set; }
    
    public Packet()
    {
    }

    public Packet(Packet parent)
    {
        OpCode = parent.OpCode;
        DispatchType = parent.DispatchType;
    }
}
