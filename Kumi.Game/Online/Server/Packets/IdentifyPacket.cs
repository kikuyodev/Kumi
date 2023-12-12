using Newtonsoft.Json;

namespace Kumi.Game.Online.Server.Packets;

public class IdentifyPacket : Packet<IdentifyPacket.IdentifyData>
{
    public OpCode OpCode => OpCode.Identify;
    
    public class IdentifyData
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
