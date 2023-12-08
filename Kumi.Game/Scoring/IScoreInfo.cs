using Kumi.Game.Charts;
using Kumi.Game.Online.API.Accounts;

namespace Kumi.Game.Scoring;

public interface IScoreInfo
{
    IAccount Account { get; }
    
    long TotalScore { get; }
    
    int MaxCombo { get; }
    
    DateTimeOffset Date { get; }
    
    IChartInfo? Chart { get; }
}
