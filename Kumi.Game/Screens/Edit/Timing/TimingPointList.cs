using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingPointList : CompositeDrawable, IKeyBindingHandler<PlatformAction>
{
    private readonly BindableList<TimingPoint> timingPoints = new BindableList<TimingPoint>();

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

        InternalChild = new KumiScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            Masking = false,
            Child = flowList = new TimingPointFlowList
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                CreatePoint = createPoint
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

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case PlatformAction.Delete:
                performDelete();
                break;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }

    private void trackActivePoint()
    {
        var nearestPoint = editorChart.TimingPoints
           .LastOrDefault(t => t.StartTime <= editorClock.CurrentTime);

        if (nearestPoint != null)
            currentPoint.Value = (TimingPoint) nearestPoint;
    }

    private void createPoint(TimingPointType type)
    {
        TimingPoint point = type switch
        {
            TimingPointType.Inherited => new InheritedTimingPoint(0),
            TimingPointType.Uninherited => new UninheritedTimingPoint(0),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        if (selectedPoint.Value != null)
            point = selectedPoint.Value.DeepClone();

        point.PointType = type;
        point.Volume = 100;
        point.RelativeScrollSpeed = 1f;
        point.StartTime = editorClock.CurrentTime;

        if (point.PointType == TimingPointType.Uninherited)
        {
            var uninherited = (UninheritedTimingPoint) point;
            uninherited.BPM = 60;
        }
        
        var closestPoint = editorChart.TimingPointHandler.GetTimingPointAt(point.StartTime);
        var idx = timingPoints.BinarySearch(closestPoint);

        if (idx == -1)
            timingPoints.Insert(0, point);
        else
            timingPoints.Insert(idx + 1, point);
    }

    private void performDelete()
    {
        if (selectedPoint.Value == null)
            return;

        timingPoints.Remove(selectedPoint.Value);
    }
}
