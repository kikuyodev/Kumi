using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Requests.Responses;

public class APIChartSet
{
    // TODO: Only the ID matters for now.
    [JsonProperty("id")]
    public int Id { get; set; }
}
