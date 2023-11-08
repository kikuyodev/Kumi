using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Judgements;
using osu.Framework.Bindables;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Gameplay.Drawables;

public partial class DrawableNote : CompositeDrawable
{
    protected INote Note { get; }

    public override bool IsPresent => base.IsPresent || (state.Value == NoteState.Idle && Clock.CurrentTime >= LifetimeStart);
    public override bool RemoveWhenNotAlive => false;
    public override bool RemoveCompletedTransforms => false;

    public event Action<DrawableNote, Judgement>? OnNewJudgement; 

    private readonly Bindable<NoteState> state = new Bindable<NoteState>();

    public IBindable<NoteState> State => state;

    public DrawableNote(INote note)
    {
        Note = note;
        AlwaysPresent = true;
        
        Judgement = new Judgement(Note);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        // Force update state to idle
        updateState(State.Value, true);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        UpdateResult(false);
    }

    #region Animations

    private void updateState(NoteState newState, bool force = false)
    {
        if (State.Value == newState && !force)
            return;

        var initialTime = Note.StartTime - InitialLifetimeOffset;

        // clear existing transforms
        base.ApplyTransformsAt(double.MinValue, true);
        base.ClearTransformsAfter(double.MinValue, true);

        using (BeginAbsoluteSequence(initialTime))
            InitialTransforms();

        using (BeginAbsoluteSequence(StateChangeTime))
            StartTimeTransforms();

        using (BeginAbsoluteSequence(HitStateUpdateTime))
            UpdateHitStateTransforms(newState);
        
        state.Value = newState;
    }

    protected virtual void InitialTransforms()
    {
    }

    protected virtual void StartTimeTransforms()
    {
    }
    
    protected virtual void UpdateHitStateTransforms(NoteState newState)
    {
    }

    public virtual double InitialLifetimeOffset => 1000;

    public double StateChangeTime => Note.StartTime;

    public double HitStateUpdateTime => Judgement.AbsoluteTime ?? Note.StartTime;

    #endregion

    #region Judgement

    public bool Judged => Judgement.HasResult;

    public Judgement Judgement { get; }

    protected void ApplyResult(NoteHitResult result)
    {
        if (Judgement.HasResult)
            throw new InvalidOperationException("Cannot apply judgement to a note that has already been judged");
        
        Judgement.ApplyResult(result, Time.Current);

        if (!Judgement.HasResult)
            throw new InvalidOperationException($"{GetType().ReadableName()} did not set a valid judgement result.");

        updateState(Judgement.IsHit ? NoteState.Hit : NoteState.Miss);

        OnNewJudgement?.Invoke(this, Judgement);
    }
    
    protected bool UpdateResult(bool userTriggered)
    {
        if (Judged)
            return false;

        CheckForResult(userTriggered, Time.Current - Note.StartTime);

        return Judged;
    }

    protected virtual void CheckForResult(bool userTriggered, double deltaTime)
    {
    }

    #endregion

}

public partial class DrawableNote<T> : DrawableNote
    where T : INote
{
    protected new T Note => (T) base.Note;

    public DrawableNote(T note)
        : base(note)
    {
    }
}
