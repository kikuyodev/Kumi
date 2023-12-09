using Kumi.Game.Gameplay.Judgements;
using Kumi.Game.Scoring;

namespace Kumi.Game.Gameplay.Scoring;

public partial class ScoreProcessor : JudgementProcessor
{
    private long totalScore;
    
    protected override void ApplyJudgementInternal(Judgement judgement)
    {
        if (IsSimulating)
            return;
        
        totalScore += judgement.GetScore();
    }

    public void PopulateScore(ScoreInfo score)
    {
        score.MaxCombo = JudgedHits;
        score.TotalScore = totalScore;
    }
}
