using Kumi.Game.Models;

namespace Kumi.Game.Charts;

/// <summary>
/// Metadata representing a chart. May be shared between multiple charts.
/// </summary>
public interface IChartMetadata
{
    /// <summary>
    /// The artist's name of the song this chart is using.
    /// </summary>
    string Artist { get; }
    
    /// <summary>
    /// Romanised version of the artist's name.
    /// </summary>
    string ArtistRomanised { get; }
    
    /// <summary>
    /// The title of the song this chart is using.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Romanised version of the song's title.
    /// </summary>
    string TitleRomanised { get; }
    
    /// <summary>
    /// The creator of this specific chart.
    /// </summary>
    RealmUser Author { get; }
    
    /// <summary>
    /// Which game, movie, or series this song is from.
    /// </summary>
    string Source { get; }
    
    /// <summary>
    /// The genre of the song, if applicable.
    /// </summary>
    string Genre { get; }
    
    /// <summary>
    /// Tags for the chart, separated by spaces.
    /// </summary>
    string Tags { get; }
    
    /// <summary>
    /// The custom preview time for this chart.
    /// If the value is -1, the song will play from 40% of the song's length.
    /// </summary>
    int PreviewTime { get; }
    
    /// <summary>
    /// The name of the audio file this chart is using.
    /// </summary>
    string AudioFile { get; }
    
    /// <summary>
    /// The name of the background image this chart is using.
    /// </summary>
    string BackgroundFile { get; }
}
