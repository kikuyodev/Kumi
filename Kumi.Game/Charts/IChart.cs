using Kumi.Game.Charts.Data;
using Kumi.Game.IO.Formats;

namespace Kumi.Game.Charts;

public interface IChart : IDecodable
{
    /// <summary>
    /// The <see cref="ChartInfo"/> of this <see cref="IChart{T}"/>.
    /// </summary>
    ChartInfo ChartInfo { get; set; }
    
    /// <summary>
    /// The <see cref="ChartMetadata"/> of this <see cref="IChart{T}"/>.
    /// </summary>
    ChartMetadata Metadata { get; }
    
    /// <summary>
    /// The <see cref="IEvent"/>s that occur throughout this <see cref="IChart{T}"/>.
    /// </summary>
    IReadOnlyList<IEvent> Events { get; }
}

public interface IChart<out T> : IChart
{
    // todo...
}
