using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Gameplay;

public abstract partial class Playfield : Container
{
    protected WorkingChart WorkingChart { get; set; }
    protected IChart Chart { get; set; } = null!;
    
    public IReadOnlyList<DrawableNote> Notes => NoteContainer.Children;

    protected readonly Container<DrawableNote> NoteContainer;
    
    protected override Container<Drawable> Content => content;

    private bool firstLoad;
    private Container content = null!;
    
    // Testing purposes
    internal Container<DrawableNote> NoteContainerInternal => NoteContainer;

    protected Playfield(WorkingChart workingChart)
    {
        RelativeSizeAxes = Axes.Both;

        NoteContainer = CreateNoteContainer().With(c =>
        {
            c.Name = "Notes";
            c.RelativeSizeAxes = Axes.Both;
        });

        WorkingChart = workingChart;
        workingChart.BeginAsyncLoad();
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
    }

    protected override void Update()
    {
        if (WorkingChart.ChartLoaded && !firstLoad)
        {
            Chart = WorkingChart.Chart;
            firstLoad = true;
            onChartLoaded();
        }

        base.Update();
    }

    public void Add(INote note)
    {
        var drawableNote = createDrawableNote(note);
        NoteContainer.Add(drawableNote);
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

    private void onChartLoaded()
    {
        foreach (var note in Chart.Notes)
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
