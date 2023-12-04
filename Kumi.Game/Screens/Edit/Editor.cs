using Kumi.Game.Charts;
using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit;

public partial class Editor : KumiScreen
{
    public override BackgroundScreen CreateBackground() => new ChartBackground();
    public override float DimAmount => 0.5f;
    public override float ParallaxAmount => 0.001f;

    private DependencyContainer dependencies = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    private EditorClock clock = null!;
    private BindableBeatDivisor beatDivisor = null!;

    private EditorScreenStack screenStack = null!;

    private readonly Bindable<WorkingChart> workingChart = new Bindable<WorkingChart>();

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> working)
    {
        workingChart.BindTarget = working;

        dependencies.CacheAs((workingChart.Value.Chart as Chart)!);

        beatDivisor = new BindableBeatDivisor();
        clock = new EditorClock(beatDivisor);
        
        dependencies.CacheAs(clock);
        dependencies.CacheAs(beatDivisor);

        clock.ChangeSource(workingChart.Value.Track);
        AddInternal(clock);

        AddRangeInternal(new Drawable[]
        {
            screenStack = new EditorScreenStack
            {
                RelativeSizeAxes = Axes.Both
            },
            new EditorOverlay
            {
                RelativeSizeAxes = Axes.Both,
                Chart = { BindTarget = workingChart }
            }
        });
    }

    [Resolved(CanBeNull = true)]
    private MusicController? musicController { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (musicController != null)
            musicController.TrackChanged += onTrackChanged;

        screenStack.Push(new ComposeScreen());
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (musicController != null)
            musicController.TrackChanged -= onTrackChanged;
    }

    private void onTrackChanged(WorkingChart working)
    {
        clock.ChangeSource(working.Track);
    }

    protected override void Update()
    {
        base.Update();
        clock.ProcessFrame();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.ControlPressed || e.AltPressed || e.SuperPressed)
            return false;

        switch (e.Key)
        {
            case Key.Left:
                clock.SeekBackward();
                return true;

            case Key.Right:
                clock.SeekForward();
                return true;
        }

        return base.OnKeyDown(e);
    }

    private double scrollAccumulation;

    protected override bool OnScroll(ScrollEvent e)
    {
        if (e.ControlPressed || e.AltPressed || e.SuperPressed)
            return false;

        var scrollComponent = e.ScrollDelta.X + e.ScrollDelta.Y;
        var scrollDirection = Math.Sign(scrollComponent);

        if (scrollAccumulation != 0 && Math.Sign(scrollAccumulation) != scrollDirection)
            scrollAccumulation = scrollDirection * (1d - Math.Abs(scrollAccumulation));

        scrollAccumulation += scrollComponent;

        while (Math.Abs(scrollAccumulation) >= 1d)
        {
            if (scrollAccumulation > 0)
                clock.SeekBackward();
            else
                clock.SeekForward();

            scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + 1d) : Math.Max(0, scrollAccumulation - 1d);
        }

        return true;
    }
}
