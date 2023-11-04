namespace Kumi.Game.Charts;

/// <summary>
/// A single chart difficulty.
/// </summary>
public interface IChartInfo : IEquatable<IChartInfo>
{
    /// <summary>
    /// The name of this chart.
    /// </summary>
    string DifficultyName { get; }
    
    /// <summary>
    /// The metadata representing this chart.
    /// </summary>
    IChartMetadata Metadata { get; }
    
    /// <summary>
    /// The initial scroll speed of this chart.
    /// All timing points that affect the scroll speed of the playfield will be relative to this value.
    /// </summary>
    float InitialScrollSpeed { get; }
    
    /// <summary>
    /// The chart set that this chart belongs to.
    /// </summary>
    IChartSetInfo? ChartSet { get; }
    
    /// <summary>
    /// The SHA-256 hash that represents the contents of this chart.
    /// </summary>
    string Hash { get; }
}
