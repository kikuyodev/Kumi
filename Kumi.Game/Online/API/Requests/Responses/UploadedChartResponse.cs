using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Requests.Responses;

public class UploadedChartResponse
{
    [JsonProperty("chart")]
    public UploadedChart Chart { get; set; } = null!;

    [JsonProperty("original_hash")]
    public string OriginalHash { get; set; } = string.Empty;
    
    public class UploadedChart
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
