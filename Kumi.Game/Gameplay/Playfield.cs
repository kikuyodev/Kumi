using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Clocks;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Gameplay;

public abstract partial class Playfield : Container
{
    protected WorkingChart WorkingChart { get; set; }
    protected IChart Chart { get; set; } = null!;

    protected readonly Container<DrawableNote> NoteContainer;

    protected override Container<Drawable> Content => content;

    private bool firstLoad;
    private Container content = null!;
    private GameplayClockContainer gameplayClockContainer = null!;

    protected Playfield(WorkingChart workingChart)
    {
        RelativeSizeAxes = Axes.Both;

        NoteContainer = new Container<DrawableNote> { Name = "Notes", RelativeSizeAxes = Axes.Both };

        WorkingChart = workingChart;
        workingChart.BeginAsyncLoad();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = gameplayClockContainer = new GameplayClockContainer(WorkingChart.Track)
        {
            RelativeSizeAxes = Axes.Both,
            Child = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    public void Reset()
    {
        gameplayClockContainer.Reset(startClock: true);
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

    private void onChartLoaded()
    {
        foreach (var note in Chart.Notes)
        {
            var drawableNote = createDrawableNote(note);
            NoteContainer.Add(drawableNote);
        }

        gameplayClockContainer.StartTime = -3000;
        gameplayClockContainer.Reset(startClock: true);
    }

    private DrawableNote createDrawableNote(INote note)
    {
        var drawableNote = CreateDrawableNote(note);
        drawableNote.LifetimeStart = note.StartTime - drawableNote.InitialLifetimeOffset;

        return drawableNote;
    }

    protected abstract DrawableNote CreateDrawableNote(INote note);
}
