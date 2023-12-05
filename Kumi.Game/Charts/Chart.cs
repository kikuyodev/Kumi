using JetBrains.Annotations;
using Kumi.Game.Charts.Events;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Timings;
using Kumi.Game.IO.Formats;
using Newtonsoft.Json;
using osu.Framework.Bindables;

namespace Kumi.Game.Charts;

public class Chart : IChart, IDecodable
{
    /// <summary>
    /// 1 - Initial Version
    /// </summary>
    public const int CURRENT_VERSION = 1;

    public ChartInfo ChartInfo { get; set; }
    public List<IEvent> Events { get; } = new List<IEvent>();
    public TimingPointHandler TimingHandler { get; } = new TimingPointHandler();
    public List<INote> Notes { get; } = new List<INote>();

    public readonly BindableList<TimingPoint> TimingPoints;

    public Chart(ChartInfo chartInfo)
        : this()
    {
        ChartInfo = chartInfo;
    }

    [UsedImplicitly]
    public Chart()
    {
        ChartInfo = new ChartInfo
        {
            Metadata = new ChartMetadata
            {
                Artist = "Unknown",
                Title = "Unknown"
            },
            DifficultyName = "Normal"
        };

        TimingPoints = TimingHandler.TimingPoints.GetBoundCopy();
    }

    [JsonIgnore]
    public ChartMetadata Metadata => ChartInfo.Metadata;

    public int Version
    {
        get => ChartInfo.ChartVersion;
        set => ChartInfo.ChartVersion = value;
    }

    public bool IsProcessed { get; internal set; }

    #region IChart implementation

    IReadOnlyList<IEvent> IChart.Events => Events;
    IReadOnlyList<INote> IChart.Notes => Notes;

    IReadOnlyList<ITimingPoint> IChart.TimingPoints => TimingHandler.TimingPoints;

    #endregion

    #region IDecodable implementation

    int IDecodable.Version
    {
        get => Version;
        set => Version = value;
    }

    bool IDecodable.IsProcessed
    {
        get => IsProcessed;
        set => IsProcessed = value;
    }

    #endregion
}
