using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Charts;

public interface IAPIChartMetadata : IAPIModal
{
    [JsonProperty("artist")]
    string Artist { get; set; }

    [JsonProperty("title")]
    string Title { get; set; }

    [JsonProperty("source")]
    string? Source { get; set; }

    [JsonProperty("tags")]
    string? Tags { get; set; }

    IApiChartRomanisedMetadata? Romanised { get; }
}

public interface IApiChartRomanisedMetadata
{
    [JsonProperty("artist_romanised")]
    string? ArtistRomanised { get; set; }

    [JsonProperty("title_romanised")]
    string? TitleRomanised { get; set; }

    [JsonProperty("source_romanised")]
    string? SourceRomanised { get; set; }
}
