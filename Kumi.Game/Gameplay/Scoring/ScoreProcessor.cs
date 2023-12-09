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
    private int goodHits;
    
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
            totalScore += calculateScore(judgement.Result, combo);
        }
    }

    public ScoreRank GetScoreRank()
    {
        var totalPossibleScore = getTotalPossibleScore();
        
        if (totalScore >= totalPossibleScore)
            return ScoreRank.Kiwami;
        else
        {
            var percentage = totalScore / totalPossibleScore;
            
            if (percentage >= 0.9)
                return ScoreRank.MiyabiPlatinum;
            else if (percentage >= 0.75)
                return ScoreRank.MiyabiGold;
            else if (percentage >= 0.65)
                return ScoreRank.IkiSilver;
            else if (percentage >= 0.45)
                return ScoreRank.IkiBronze;
            else
                return ScoreRank.Iki;
        }
    }

    public void PopulateScore(ScoreInfo score)
    {
        score.MaxCombo = highestCombo;
        score.TotalScore = totalScore;
        score.ScoreRank = GetScoreRank();
        score.ComboRank = (goodHits == MaxHits) ? ScoreComboRank.PerfectCombo
                              : (highestCombo == MaxHits) ? ScoreComboRank.FullCombo
                                : ScoreComboRank.Clear;
    }

    private long getTotalPossibleScore()
    {
        var score = 0L;
        
        for (var i = 0; i < MaxHits; i++)
        {
            score += calculateScore(NoteHitResult.Good, i);
        }
        
        return score;
    }
    
    private long calculateScore(NoteHitResult result, int currentCombo) => getScoreValueFor(result) + Int64.Max(0, (long)Math.Floor(Math.Min(currentCombo, 100) / 10.0));
    
    private long getScoreValueFor(NoteHitResult result) => result switch
    {
        NoteHitResult.Good => good_score,
        NoteHitResult.Ok => ok_score,
        NoteHitResult.Bad => bad_score,
        _ => 0
    };
}
