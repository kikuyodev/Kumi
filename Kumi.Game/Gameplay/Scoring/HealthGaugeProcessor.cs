using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Judgements;

namespace Kumi.Game.Gameplay.Scoring;

public partial class HealthGaugeProcessor : JudgementProcessor
{
    private const float good_health = 0.01f;
    private const float ok_health = 0.005f;
    private const float bad_health = 0.0f;
    
    public bool HasFailed { get; private set; }
    
    private float health;
    private float maxHealth;

    protected override void ApplyJudgementInternal(Judgement judgement)
    {
        health += getHealthIncrementFor(judgement.Result);
        health = Math.Min(1, Math.Max(health, 0));
    }

    public override void ApplyChart(IChart chart)
    {
        base.ApplyChart(chart);
        
        maxHealth = MaxHits * good_health;
    }
    
    public float GetHealthPercentage() => health / maxHealth;
    
    protected override void Reset(bool storeResults)
    {
        base.Reset(storeResults);

        if (storeResults)
        {
            maxHealth = 0;
        }

        health = 0;
    }

    private float getHealthIncrementFor(NoteHitResult result) => result switch
    {
        NoteHitResult.Good => good_health,
        NoteHitResult.Ok => ok_health,
        NoteHitResult.Bad => bad_health,
        _ => -0.01f // misses and other results count as a miss
    };
}
