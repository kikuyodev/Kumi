using System.ComponentModel;

namespace Kumi.Game.Online.API.Charts;

public enum APIChartSetStatus
{
    [Description("WIP")]
    WorkInProgress,
    Pending,
    Ranked,
    Qualified,
    Graveyard
}
