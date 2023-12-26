using System.ComponentModel;

namespace Kumi.Game.Scoring;

public enum ScoreRank
{
    [Description("Iki")]
    Iki,

    [Description("Iki+")]
    IkiBronze,

    [Description("Iki++")]
    IkiSilver,

    [Description("Miyabi")]
    MiyabiGold,

    [Description("Miyabi+")]
    MiyabiPlatinum,

    [Description("Kiwami")]
    Kiwami
}
