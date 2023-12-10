using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kumi.Game.Online.Server;

[JsonConverter(typeof(StringEnumConverter))]
public enum OpCode
{
    Hello = 0x00,
    Identify = 0x01,
}
