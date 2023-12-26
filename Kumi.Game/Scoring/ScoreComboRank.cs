using System.ComponentModel;

namespace Kumi.Game.Scoring;

public enum ScoreComboRank
{
    [Description("")]
    Clear,
    
    [Description("FC")]
    FullCombo,
    
    [Description("PC")]
    PerfectCombo
}
