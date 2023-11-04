using JetBrains.Annotations;
using Kumi.Game.Database;
using Kumi.Game.Models;
using Newtonsoft.Json;
using NuGet.Packaging;
using Realms;

namespace Kumi.Game.Charts;

[MapTo("ChartSet")]
public class ChartSetInfo : RealmObject, IHasGuidPrimaryKey, IChartSetInfo, ISoftDelete, IEquatable<ChartSetInfo>
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public DateTimeOffset DateAdded { get; set; }

    [JsonIgnore]
    [Ignored]
    public ChartMetadata Metadata => Charts.FirstOrDefault()?.Metadata ?? new ChartMetadata();
    
    public IList<ChartInfo> Charts { get; } = null!;
    public IList<RealmNamedFileUsage> Files { get; } = null!;

    [Ignored]
    public RealmUser Creator => Metadata.Creator;

    public string Hash { get; set; } = string.Empty;
    
    public bool DeletePending { get; set; }

    public ChartSetInfo(IEnumerable<ChartInfo>? charts = null)
    {
        ID = Guid.NewGuid();
        if (charts != null)
            Charts.AddRange(charts);
    }

    [UsedImplicitly]
    private ChartSetInfo()
    {
    }

    public bool Equals(ChartSetInfo? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        
        return ID == other.ID;
    }

    public bool Equals(IChartSetInfo? other) => other is ChartSetInfo c && Equals(c);

    #region IChartSetInfo implementation

    IChartMetadata IChartSetInfo.Metadata => Metadata;
    IEnumerable<IChartInfo> IChartSetInfo.Charts => Charts;

    #endregion
}

