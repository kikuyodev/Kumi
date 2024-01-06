using Newtonsoft.Json;

namespace Kumi.Game.Online.API;

public interface IAPIModal
{
    [JsonProperty("id")]
    int Id { get; set; }

    [JsonProperty("created_at")]
    DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    DateTime UpdatedAt { get; set; }
}
