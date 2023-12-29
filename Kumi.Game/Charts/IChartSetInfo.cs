using Kumi.Game.Database;
using Kumi.Game.Online;

namespace Kumi.Game.Charts;

/// <summary>
/// A representation of a collection of charts.
/// </summary>
public interface IChartSetInfo : IHasFiles, IHasOnlineID, IEquatable<IChartSetInfo>
{
    /// <summary>
    /// The date and time this set was added to the database.
    /// </summary>
    DateTimeOffset DateAdded { get; }

    /// <summary>
    /// The metadata representing this set.
    /// </summary>
    /// <remarks>
    /// It's not guaranteed that this metadata will be the same metadata for the large majority of charts in this set.
    /// Only the first chart in the set is used to determine the metadata.
    /// </remarks>
    IChartMetadata Metadata { get; }

    /// <summary>
    /// The collection of charts in this set.
    /// </summary>
    IEnumerable<IChartInfo> Charts { get; }
}
