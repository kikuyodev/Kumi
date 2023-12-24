using Kumi.Game.Charts;
using Kumi.Game.Gameplay.Mods;
using Kumi.Game.Input;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Edit.Compose;
using Kumi.Game.Screens.Edit.Popup;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK.Graphics;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit;

public partial class Editor : ScreenWithChartBackground, IKeyBindingHandler<PlatformAction>, IKeyBindingHandler<GlobalAction>
{
    protected override OverlayActivation InitialOverlayActivation => Overlays.OverlayActivation.UserTriggered;
    public override float DimAmount => 0.5f;
    public override float ParallaxAmount => 0.001f;
    public override bool AllowBackButton => false;

    private DependencyContainer dependencies = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;
    
    [Resolved]
    private Bindable<WorkingChart> chart { get; set; } = null!;
    
    [Resolved]
    private BindableList<Mod> mods { get; set; } = null!;
    
    [Resolved]
    private KumiGameBase game { get; set; } = null!;

    private EditorClock clock = null!;
    private BindableBeatDivisor beatDivisor = null!;

    private EditorScreenStack screenStack = null!;

    private readonly Bindable<WorkingChart> workingChart = new Bindable<WorkingChart>();
    private readonly BindableBool hasUnsavedChanges = new BindableBool();

    private EditorChart editorChart = null!;
    private EditorHistoryHandler historyHandler = null!;

    public readonly Bindable<EditorScreen> CurrentScreen = new Bindable<EditorScreen>();
    
    private string lastSavedHash = string.Empty;
    
    private Box popupBackground = null!;
    private ExitWithoutSavingPopup exitWithoutSavingPopup = null!;
    
    private readonly List<EditorPopup> openPopups = new List<EditorPopup>();
    
    private readonly List<Mod> previouslyAppliedMods = new List<Mod>();
    
    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> working)
    {
        previouslyAppliedMods.AddRange(mods);
        mods.Clear();
        
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

        historyHandler.OnStateChange += () =>
        {
            hasUnsavedChanges.Value = lastSavedHash != historyHandler.CurrentStateHash;
        };

        clock.ChangeSource(workingChart.Value.Track);
        AddInternal(clock);

        AddRangeInternal(new Drawable[]
        {
            screenStack = new EditorScreenStack
            {
                RelativeSizeAxes = Axes.Both
            },
            new EditorOverlay(hasUnsavedChanges)
            {
                RelativeSizeAxes = Axes.Both,
                Chart = { BindTarget = workingChart }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    popupBackground = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black.Opacity(0.5f),
                        Alpha = 0
                    },
                    exitWithoutSavingPopup = new ExitWithoutSavingPopup
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = this.Exit
                    }
                }
            }
        });
        
        exitWithoutSavingPopup.State.BindValueChanged(v =>
        {
            if (v.NewValue == Visibility.Visible)
                openPopups.Add(exitWithoutSavingPopup);
            else
                openPopups.Remove(exitWithoutSavingPopup);

            Schedule(updatePopupBackground);
        });

        screenStack.ScreenPushed += onScreenChanged;
        screenStack.ScreenExited += onScreenChanged;
    }
    
    private void updatePopupBackground()
    {
        popupBackground.FadeTo(openPopups.Count > 0 ? 1 : 0, 200, Easing.OutQuint);
    }

    private void onScreenChanged(IScreen current, IScreen newScreen)
    {
        CurrentScreen.Value = (EditorScreen) newScreen;
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

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        clock.Seek(0);
        clock.Stop();
        workingChart.Disabled = true;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        workingChart.Disabled = false;
        refetchChart();
        
        previouslyAppliedMods.ForEach(m => mods.Add(m));

        return base.OnExiting(e);
    }

    private void refetchChart()
    {
        var refetched = chartManager.GetWorkingChart(editorChart.ChartInfo, true);

        if (!(refetched is DummyWorkingChart))
            chart.Value = refetched;
    }

    public void AttemptExit()
    {
        if (hasUnsavedChanges.Value)
            exitWithoutSavingPopup.Show();
        else
            this.Exit();
    }

    protected override void Update()
    {
        base.Update();
        clock.ProcessFrame();
    }

    private void onTrackChanged(WorkingChart working)
    {
        clock.ChangeSource(working.Track);
    }

    public void PushScreen(EditorScreen screen)
    {
        LoadComponentAsync(screen, s => screenStack.Push(s));
    }

    public void RefreshBackground()
    {
        workingChart.TriggerChange();
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

            case PlatformAction.Save:
                if (e.Repeat)
                    return false;

                Save();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case GlobalAction.Back:
                AttemptExit();
                break;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
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

    public bool Save()
    {
        try
        {
            chartManager.Save(editorChart.ChartInfo, (Chart) editorChart.PlayableChart);
            lastSavedHash = historyHandler.CurrentStateHash;
            hasUnsavedChanges.Value = lastSavedHash != historyHandler.CurrentStateHash;
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            return false;
        }

        return true;
    }

    public async void Export()
    {
        Save(); // ensure we're exporting the latest version of the chart.
        var path = await chartManager.Export(editorChart.ChartInfo.ChartSet!).ConfigureAwait(false);
        
        if (string.IsNullOrEmpty(path))
            return;
        
        var storage = game.Storage!.GetStorageForDirectory("export");
        storage.PresentFileExternally(path);
    }

    private void seek(UIEvent e, int direction)
    {
        var amount = e.ShiftPressed ? 4 : 1;

        if (direction < 1)
            clock.SeekBackward(amount);
        else
            clock.SeekForward(amount);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (musicController != null)
            musicController.TrackChanged -= onTrackChanged;
    }
}
