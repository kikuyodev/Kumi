using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kumi.Game.Online.Server;

[JsonConverter(typeof(StringEnumConverter))]
public enum OpCode
{
    Dispatch = 0x00,
    Hello = 0x01,
    Identify = 0x02,
}
