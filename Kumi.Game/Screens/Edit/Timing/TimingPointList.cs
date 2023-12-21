using Kumi.Game.Charts.Timings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingPointList : CompositeDrawable
{
    private readonly IBindableList<TimingPoint> timingPoints = new BindableList<TimingPoint>();

    private TimingPointFlowList flowList = null!;

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    [Resolved(name: "current_point")]
    private Bindable<TimingPoint> currentPoint { get; set; } = null!;

    [Cached(name: "selected_point")]
    private Bindable<TimingPoint?> selectedPoint = new Bindable<TimingPoint?>();

    [Resolved]
    private EditorHistoryHandler? historyHandler { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChild = new BasicScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            Masking = false,
            Child = flowList = new TimingPointFlowList
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        timingPoints.BindTo(editorChart.TimingPointHandler.TimingPoints);
        timingPoints.BindCollectionChanged((_, _) =>
        {
            flowList.TimingPoints = timingPoints;
            historyHandler?.SaveState();
        }, true);
    }

    protected override bool OnClick(ClickEvent e)
    {
        selectedPoint.Value = null;
        return true;
    }

    protected override void Update()
    {
        base.Update();

        trackActivePoint();
    }

    private void trackActivePoint()
    {
        var nearestPoint = editorChart.TimingPoints
           .LastOrDefault(t => t.StartTime <= editorClock.CurrentTime);

        if (nearestPoint != null)
            currentPoint.Value = (TimingPoint) nearestPoint;
    }
}
