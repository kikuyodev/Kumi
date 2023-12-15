using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Judgements;
using osu.Framework.Bindables;

namespace Kumi.Game.Gameplay.Scoring;

public partial class HealthGaugeProcessor : JudgementProcessor
{
    private const float good_health = 0.01f;
    private const float ok_health = 0.005f;
    private const float bad_health = 0.0f;

    public bool HasFailed { get; private set; }

    private float health;
    private float maxHealth;

    private readonly Bindable<double> healthBindable = new BindableDouble();

    public IBindable<double> Health => healthBindable;

    protected override void ApplyJudgementInternal(Judgement judgement)
    {
        if (judgement.IsBonus)
            return;
        
        health += getHealthIncrementFor(judgement.Result);
        health = Math.Min(1, Math.Max(health, 0));

        if (!IsSimulating)
            healthBindable.Value = health / maxHealth;
    }

    protected override void Reset(bool storeResults)
    {
        base.Reset(storeResults);

        if (storeResults)
            maxHealth = MaxHits * good_health;

        health = 0;
    }

    private float getHealthIncrementFor(NoteHitResult result) => result switch
    {
        NoteHitResult.Good => good_health,
        NoteHitResult.Ok => ok_health,
        NoteHitResult.Bad => bad_health,
        _ => -0.005f // misses and other results count as a miss
    };
}
