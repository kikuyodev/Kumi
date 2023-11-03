using Kumi.Game.Charts.Data;
using Kumi.Game.Charts.Timings;
using Kumi.Game.IO.Formats;
using Newtonsoft.Json;

namespace Kumi.Game.Charts;

public class Chart<T> : IChart<T>
{
    public ChartInfo ChartInfo { get; set; }

    public List<IEvent> Events { get; } = new List<IEvent>();
    
    public TimingPointHandler TimingPoints { get; } = new TimingPointHandler();

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
    }

    [JsonIgnore]
    public ChartMetadata Metadata => ChartInfo.Metadata;

    public int Version { get; internal set; }
    public bool IsProcessed { get; internal set; }

    #region IChart implementation
    
    IReadOnlyList<IEvent> IChart.Events => Events;

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
