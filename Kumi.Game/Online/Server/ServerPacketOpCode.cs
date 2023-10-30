using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kumi.Game.Online.Server;

[JsonConverter(typeof(StringEnumConverter))]
public enum ServerPacketOpCode
{
    Hello = 0x00,
}
