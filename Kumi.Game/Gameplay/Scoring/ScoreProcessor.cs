using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Extensions;
using Kumi.Game.Gameplay.Judgements;
using Kumi.Game.Scoring;

namespace Kumi.Game.Gameplay.Scoring;

public partial class ScoreProcessor : JudgementProcessor
{
    private const long good_score = 300;
    private const long ok_score = 200;
    private const long bad_score = 100;
    
    private long totalScore;
    private int combo;
    private int highestCombo;
    
    protected override void ApplyJudgementInternal(Judgement judgement)
    {
        if (IsSimulating)
            return;

        if (judgement.Result == NoteHitResult.Miss)
        {
            if (combo > highestCombo)
                highestCombo = combo;
            
            combo = 0;
        } else 
            combo++;

        if (judgement.Result.AffectsScore())
        {
            totalScore += getScoreValueFor(judgement.Result) + Int64.Max(0, (long)Math.Floor(Math.Min(combo, 100) / 10.0));
        }
    }

    public void PopulateScore(ScoreInfo score)
    {
        score.MaxCombo = highestCombo;
        score.TotalScore = totalScore;
    }
    
    private long getScoreValueFor(NoteHitResult result) => result switch
    {
        NoteHitResult.Good => good_score,
        NoteHitResult.Ok => ok_score,
        NoteHitResult.Bad => bad_score,
        _ => 0
    };
}
