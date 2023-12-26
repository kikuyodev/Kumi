using JetBrains.Annotations;
using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Game.Models;
using Kumi.Game.Online.API.Accounts;
using Realms;

namespace Kumi.Game.Scoring;

[MapTo("Score")]
public class ScoreInfo : RealmObject, IHasGuidPrimaryKey, ISoftDelete, IHasFiles, IEquatable<ScoreInfo>, IScoreInfo
{
    [PrimaryKey]
    public Guid ID { get; set; }
    
    public ChartInfo? ChartInfo { get; set; }
    
    public string ChartHash { get; set; } = string.Empty;

    public IList<RealmNamedFileUsage> Files { get; } = null!;
    
    public string Hash { get; set; } = string.Empty;
    
    public bool DeletePending { get; set; }

    public long TotalScore { get; set; }

    public int MaxCombo { get; set; }

    public ScoreStatistics Statistics { get; set; } = null!;
    
    [Ignored]
    public ScoreRank ScoreRank
    {
        get => (ScoreRank) ScoreRankInt;
        set => ScoreRankInt = (int) value;
    }
    
    [Ignored]
    public ScoreComboRank ComboRank
    {
        get => (ScoreComboRank) ComboRankInt;
        set => ComboRankInt = (int) value;
    }
    
    public bool Failed { get; set; }

    public DateTimeOffset Date { get; set; }

    [MapTo("User")]
    public RealmAccount RealmAccount { get; set; } = null!;
    
    
    [MapTo(nameof(ScoreRank))]
    
    public int ScoreRankInt { get; set; }
    [MapTo(nameof(ComboRank))]
    public int ComboRankInt { get; set; }

    public ScoreInfo(ChartInfo? chart = null, RealmAccount? realmAccount = null)
    {
        ID = Guid.NewGuid();

        ChartInfo = chart ?? new ChartInfo();
        ChartHash = ChartInfo.Hash;
        RealmAccount = realmAccount ?? new RealmAccount();
        Statistics = new ScoreStatistics();
    }
    
    [UsedImplicitly]
    private ScoreInfo()
    {
    }

    private APIAccount? account;

    [Ignored]
    public APIAccount Account
    {
        get => account ??= new APIAccount
        {
            Id = RealmAccount.Id,
            Username = RealmAccount.Username
        };
        set
        {
            account = value;

            RealmAccount = new RealmAccount
            {
                Id = account.Id,
                Username = account.Username
            };
        }
    }

    IChartInfo? IScoreInfo.Chart => ChartInfo;
    IAccount IScoreInfo.Account => Account;

    public ScoreInfo DeepClone()
    {
        var clone = (ScoreInfo)this.Detach().MemberwiseClone();

        clone.RealmAccount = new RealmAccount
        {
            Id = RealmAccount.Id,
            Username = RealmAccount.Username
        };

        return clone;
    }

    public bool Equals(ScoreInfo? other) => other?.ID == ID;
}
