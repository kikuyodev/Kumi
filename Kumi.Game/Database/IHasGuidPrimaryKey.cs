using Newtonsoft.Json;
using Realms;

namespace Kumi.Game.Database;

public interface IHasGuidPrimaryKey
{
    [JsonIgnore]
    [PrimaryKey]
    Guid ID { get; }
}
