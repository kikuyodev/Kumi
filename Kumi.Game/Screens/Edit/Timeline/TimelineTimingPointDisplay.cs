using System.Collections.Specialized;
using System.Diagnostics;
using Kumi.Game.Charts.Timings;
using Kumi.Game.Screens.Edit.Timeline.Parts;
using osu.Framework.Bindables;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineTimingPointDisplay : TimelinePart<TimelineTimingPoint>
{
    private readonly IBindableList<TimingPoint> timingPoints = new BindableList<TimingPoint>();

    protected override void LoadChart(EditorChart editorChart)
    {
        base.LoadChart(editorChart);

        timingPoints.UnbindAll();
        timingPoints.BindTo(editorChart.TimingPointHandler.TimingPoints);
        timingPoints.BindCollectionChanged(timingPointsChanged, true);
    }

    private void timingPointsChanged(object? _, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                Clear();
                break;

            case NotifyCollectionChangedAction.Add:
                Debug.Assert(args.NewItems != null);

                foreach (var point in args.NewItems.Cast<TimingPoint>())
                    Add(new TimelineTimingPoint(point));
                break;

            case NotifyCollectionChangedAction.Remove:
                Debug.Assert(args.OldItems != null);

                foreach (var point in args.OldItems.Cast<TimingPoint>())
                {
                    var matching = Children.SingleOrDefault(p => ReferenceEquals(p.Point, point));
                    matching?.Expire();
                }

                break;
        }
    }
}
