using Kumi.Game.Charts;
using Kumi.Game.Charts.Events;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Timings;
using Kumi.Game.IO.Formats;
using Newtonsoft.Json;
using osu.Framework.Bindables;

namespace Kumi.Game.Charts;

public class Chart<T> : IChart<T>
{
    public ChartInfo ChartInfo { get; set; }
    public List<IEvent> Events { get; } = new List<IEvent>();
    public TimingPointHandler TimingHandler { get; } = new TimingPointHandler();
    public List<Note> Notes { get; } = new List<Note>();

    public BindableList<TimingPoint> TimingPoints;

    protected Chart()
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

    public int Version { get; internal set; }
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

public class Chart : Chart<dynamic>
{
}
