using Kumi.Game.Models;
using Newtonsoft.Json;
using Realms;

namespace Kumi.Game.Charts;

public class ChartMetadata : RealmObject, IChartMetadata
{
    public string Artist { get; set; } = string.Empty;

    [JsonProperty("artist_romanised")]
    public string ArtistRomanised { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("title_romanised")]
    public string TitleRomanised { get; set; } = string.Empty;

    public RealmUser Author { get; set; } = null!;

    public string Source { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;

    public int PreviewTime { get; set; } = -1;

    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;
}

