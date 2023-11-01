using Newtonsoft.Json;

namespace Kumi.Game.Charts;

public class Chart<T> : IChart<T>
{
    public ChartInfo ChartInfo { get; set; }

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
    }

    [JsonIgnore]
    public ChartMetadata Metadata => ChartInfo.Metadata;
}

public class Chart : Chart<dynamic>
{
}
