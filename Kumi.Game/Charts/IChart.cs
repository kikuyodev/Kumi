namespace Kumi.Game.Charts;

public interface IChart
{
    /// <summary>
    /// The <see cref="ChartInfo"/> of this <see cref="IChart{T}"/>.
    /// </summary>
    ChartInfo ChartInfo { get; set; }
    
    /// <summary>
    /// The <see cref="ChartMetadata"/> of this <see cref="IChart{T}"/>.
    /// </summary>
    ChartMetadata Metadata { get; }
    
    // todo...
}

public interface IChart<out T> : IChart
{
    // todo...
}
