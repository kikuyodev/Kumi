using Kumi.Game.Online.API.Accounts;
using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Charts;

public class APIChart : IAPIChartMetadata
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

    [JsonProperty("difficulty_name")]
    public string DifficultyName { get; set; } = string.Empty;

    [JsonProperty("creators")]
    public APIAccount[] Creators { get; set; } = null!;

    [JsonProperty("difficulty")]
    public APIChartDifficulty Difficulty { get; set; } = null!;

    [JsonProperty("statistics")]
    public APIChartStatistics Statistics { get; set; } = null!;

    [JsonProperty("romanised_metadata")]
    public APIRomanisedMetadata? Romanised { get; set; }

    IApiChartRomanisedMetadata? IAPIChartMetadata.Romanised => Romanised;
}

public class APIChartDifficulty
{
    [JsonProperty("bpms")]
    public float[] BPMs { get; set; }

    [JsonProperty("difficulty")]
    public float Difficulty { get; set; }
}

public class APIChartStatistics
{
    [JsonProperty("note_count")]
    public int NoteCount { get; set; }

    [JsonProperty("drain_length")]
    public float DrainLength { get; set; }

    [JsonProperty("total_length")]
    public float TotalLength { get; set; }
    
    [JsonProperty("music_length")]
    public float MusicLength { get; set; }
}
