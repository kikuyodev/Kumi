using Kumi.Game.Charts;
using Kumi.Game.Gameplay;
using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit;

public partial class EditorScreen : KumiScreen
{
    public override BackgroundScreen CreateBackground() => new ChartBackground();
    public override float DimAmount => 0.5f;
    public override float ParallaxAmount => 0.001f;

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    private readonly Bindable<WorkingChart> chart = new Bindable<WorkingChart>();

    [Cached]
    private EditorClock clock = new EditorClock();

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> working)
    {
        chart.BindTarget = working;

        clock.ChangeSource(chart.Value.Track);
        AddInternal(clock);

        AddInternal(new EditorOverlay
        {
            RelativeSizeAxes = Axes.Both,
            Chart = { BindTarget = chart }
        });
    }

    [Resolved(CanBeNull = true)]
    private MusicController? musicController { get; set; }

    private ScheduledDelegate? playfieldLoad;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (musicController != null)
            musicController.TrackChanged += onTrackChanged;

        playfieldLoad = Scheduler.AddDelayed(() =>
        {
            if (!chart.Value.ChartLoaded)
                return;

            AddInternal(new InputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1,
                Child = new KumiPlayfield(chart.Value)
                {
                    Clock = clock,
                    ProcessCustomClock = false
                }
            });

            playfieldLoad?.Cancel();
            playfieldLoad = null;
        }, 1, true);
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
