using Kumi.Game.Charts;
using Kumi.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit;

public partial class Editor : ScreenWithChartBackground, IKeyBindingHandler<PlatformAction>
{
    public override float DimAmount => 0.5f;
    public override float ParallaxAmount => 0.001f;
    public override bool ShowTaskbar => false;
    public override bool DisableTaskbarControl => true;

    private DependencyContainer dependencies = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    private EditorClock clock = null!;
    private BindableBeatDivisor beatDivisor = null!;

    private EditorScreenStack screenStack = null!;

    private readonly Bindable<WorkingChart> workingChart = new Bindable<WorkingChart>();

    private EditorChart editorChart = null!;
    private EditorHistoryHandler historyHandler = null!;

    public Bindable<EditorScreen> CurrentScreen = new Bindable<EditorScreen>();

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> working)
    {
        workingChart.BindTarget = working;
        
        dependencies.CacheAs(this);
        dependencies.CacheAs((workingChart.Value.Chart as Chart)!);

        beatDivisor = new BindableBeatDivisor();
        clock = new EditorClock(beatDivisor);

        dependencies.CacheAs(clock);
        dependencies.CacheAs(beatDivisor);

        AddInternal(editorChart = new EditorChart(working.Value.Chart, working.Value.ChartInfo));
        dependencies.CacheAs(editorChart);

        AddInternal(historyHandler = new EditorHistoryHandler(editorChart));
        dependencies.CacheAs(historyHandler);

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
        
        screenStack.ScreenPushed += onScreenChanged;
        screenStack.ScreenExited += onScreenChanged;
    }

    private void onScreenChanged(IScreen current, IScreen newScreen)
    {
        CurrentScreen.Value = (EditorScreen)newScreen;
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

    public void Undo() => historyHandler.RestoreState(-1);
    public void Redo() => historyHandler.RestoreState(1);

    public void Copy(bool cut)
        => screenStack.CurrentScreen.Copy(cut);
    
    public void Paste()
        => screenStack.CurrentScreen.Paste();

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        switch (e.Action)
        {
            case PlatformAction.Undo:
                Undo();
                return true;

            case PlatformAction.Redo:
                Redo();
                return true;
            
            case PlatformAction.Cut:
                Copy(true);
                return true;
            
            case PlatformAction.Copy:
                Copy(false);
                return true;
            
            case PlatformAction.Paste:
                Paste();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.ControlPressed || e.AltPressed || e.SuperPressed)
            return false;

        switch (e.Key)
        {
            case Key.Left:
                seek(e, -1);
                return true;

            case Key.Right:
                seek(e, -1);
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
                seek(e, -1);
            else
                seek(e, 1);

            scrollAccumulation = scrollAccumulation < 0 ? Math.Min(0, scrollAccumulation + 1d) : Math.Max(0, scrollAccumulation - 1d);
        }

        return true;
    }

    private void seek(UIEvent e, int direction)
    {
        var amount = e.ShiftPressed ? 4 : 1;

        if (direction < 1)
            clock.SeekBackward(amount);
        else
            clock.SeekForward(amount);
    }
}
