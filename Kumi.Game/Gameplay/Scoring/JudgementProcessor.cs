using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Judgements;
using osu.Framework.Bindables;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;

namespace Kumi.Game.Gameplay.Scoring;

public abstract partial class JudgementProcessor : Component
{
    public event Action<Judgement>? NewJudgement;

    protected int MaxHits { get; private set; }

    protected bool IsSimulating { get; private set; }

    public int JudgedHits { get; private set; }

    private Judgement? lastAppliedJudgement;

    private readonly BindableBool hasCompleted = new BindableBool();

    public IBindable<bool> HasCompleted => hasCompleted;

    public void ApplyChart(IChart chart)
    {
        Reset(false);
        simulateJudgements(chart);
        Reset(true);
    }

    public void ApplyJudgement(Judgement judgement)
    {
        if (!judgement.IsBonus)
            JudgedHits++;

        lastAppliedJudgement = judgement;
        ApplyJudgementInternal(judgement);
        NewJudgement?.Invoke(judgement);
    }

    protected abstract void ApplyJudgementInternal(Judgement judgement);

    protected virtual void Reset(bool storeResults)
    {
        if (storeResults)
            MaxHits = JudgedHits;

        JudgedHits = 0;
    }

    private void simulateJudgements(IChart chart)
    {
        IsSimulating = true;

        foreach (var note in chart.Notes)
        {
            var judgement = new Judgement(note);
            judgement.ApplyResult(NoteHitResult.Good, note.StartTime);
            ApplyJudgement(judgement);
        }

        IsSimulating = false;
    }

    protected override void Update()
    {
        base.Update();

        hasCompleted.Value = JudgedHits == MaxHits && (JudgedHits == 0 || lastAppliedJudgement.AsNonNull().AbsoluteTime < Clock.CurrentTime);
    }
}
