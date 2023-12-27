using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Gameplay.Judgements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Gameplay;

public abstract partial class Playfield : Container
{
    public event Action<DrawableNote, Judgement>? NewJudgement; 
    
    protected WorkingChart WorkingChart { get; set; }
    protected IChart Chart { get; set; }

    public IReadOnlyList<DrawableNote> Notes => NoteContainer.Children;

    protected readonly Container<DrawableNote> NoteContainer;

    protected override Container<Drawable> Content => content;

    private Container content = null!;

    // Testing purposes
    internal Container<DrawableNote> NoteContainerInternal => NoteContainer;

    private readonly Stack<DrawableNote> judgedNotes;

    protected Playfield(WorkingChart workingChart)
    {
        RelativeSizeAxes = Axes.Both;

        NoteContainer = CreateNoteContainer().With(c =>
        {
            c.Name = "Notes";
            c.RelativeSizeAxes = Axes.Both;
        });

        WorkingChart = workingChart;
        Chart = WorkingChart.Chart;
        
        judgedNotes = new Stack<DrawableNote>();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };

        addNotes();
    }

    protected override void Update()
    {
        base.Update();

        while (judgedNotes.Count > 0)
        {
            var judgement = judgedNotes.Peek().Judgement;
            
            if (Time.Current >= judgement.RawTime!.Value)
                break;

            revertResult(judgedNotes.Pop());
        }
    }

    // Reverts the judgement for seeking purposes.
    private void revertResult(DrawableNote note)
    {
        note.OnRevertResult();
        note.Judgement.Reset();
    }

    public DrawableNote Add(INote note)
    {
        var drawableNote = createDrawableNote(note);
        NoteContainer.Add(drawableNote);
        
        drawableNote.OnNewJudgement += onNewResult;
        return drawableNote;
    }

    private void onNewResult(DrawableNote note, Judgement judgement)
    {
        judgedNotes.Push(note);
        NewJudgement?.Invoke(note, judgement);
    }

    public bool Remove(INote note)
    {
        var index = NoteContainer.IndexOf(NoteContainer.Children.FirstOrDefault(n => n.Note == note)!);
        if (index != -1)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        var drawableNote = NoteContainer.ElementAtOrDefault(index);
        if (drawableNote != null)
            NoteContainer.Remove(drawableNote, true);
    }

    public void SetKeepAlive(INote note, bool keepAlive)
    {
        var drawableNote = NoteContainer.Children.FirstOrDefault(n => n.Note == note);
        if (drawableNote != null)
            drawableNote.AlwaysPresent = keepAlive;
    }

    private void addNotes()
    {
        var sortedNotes = Chart.Notes.OrderBy(n => n.StartTime).ToList();
        
        foreach (var note in sortedNotes)
            Add(note);
    }

    private DrawableNote createDrawableNote(INote note)
    {
        var drawableNote = CreateDrawableNote(note);
        drawableNote.LifetimeStart = ComputeInitialLifetimeOffset(drawableNote);

        return drawableNote;
    }

    public virtual void UpdateLifetime(Note note)
    {
        var drawableNote = NoteContainer.Children.FirstOrDefault(n => n.Note == note);
        if (drawableNote != null)
        {
            drawableNote.LifetimeStart = ComputeInitialLifetimeOffset(drawableNote);

            // Recompute transforms
            drawableNote.Reset();
        }
    }

    public void UpdateLifetimeRange(IEnumerable<Note> notes)
    {
        foreach (var note in notes)
            UpdateLifetime(note);
    }

    protected virtual double ComputeInitialLifetimeOffset(DrawableNote drawableNote)
        => drawableNote.Note.StartTime - drawableNote.InitialLifetimeOffset;

    protected abstract DrawableNote CreateDrawableNote(INote note);

    protected virtual Container<DrawableNote> CreateNoteContainer() => new Container<DrawableNote>();
}
