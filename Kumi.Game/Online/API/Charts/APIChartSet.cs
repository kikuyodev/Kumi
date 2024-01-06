using Kumi.Game.Online.API.Accounts;
using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Charts;

public class APIChartSet : IAPIChartMetadata
{
    #region IAPIModal

    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    #endregion

    #region IAPIChartMetadata

    public string Artist { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Source { get; set; }

    public string? Tags { get; set; }

    #endregion

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("charts")]
    public APIChart[] Charts { get; set; } = null!;

    [JsonProperty("creator")]
    public APIAccount Creator { get; set; } = null!;

    [JsonProperty("nominators")]
    public APIAccount[] Nominators { get; set; } = null!;

    [JsonProperty("status")]
    public APIChartSetStatus Status { get; set; }

    [JsonProperty("ranked_on")]
    public DateTime? RankedOn { get; set; }

    [JsonProperty("attributes")]
    public APIChartSetAttributes Attributes { get; set; } = null!;

    [JsonProperty("romanised_metadata")]
    public APIRomanisedMetadata? Romanised { get; set; }

    IApiChartRomanisedMetadata? IAPIChartMetadata.Romanised => Romanised;
}

public class APIChartSetAttributes
{
    [JsonProperty("is_unavailable")]
    public bool IsUnavailable { get; set; }

    [JsonProperty("unavailable_reason")]
    public string? UnavailableReason { get; set; }
}
