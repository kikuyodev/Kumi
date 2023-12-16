using Kumi.Game.Charts.Objects;

namespace Kumi.Game.Gameplay.Judgements;

public class BarLineJudgement : Judgement
{
    public BarLineJudgement(INote note, bool isBonus = false)
        : base(note, isBonus)
    {
        Ignored = true;
    }
}
