using System.Collections.Specialized;
using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Timings;
using osu.Framework.Bindables;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class TimingPointsPart : TimelinePart<DrawableTimingPoint>
{
    private readonly IBindableList<TimingPoint> timingPoints = new BindableList<TimingPoint>();
    
    protected override void LoadChart(IChart chart)
    {
        base.LoadChart(chart);
        
        timingPoints.UnbindAll();
        timingPoints.BindTo(((Chart) chart).TimingPoints);
        timingPoints.BindCollectionChanged((_, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
                
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(args.NewItems != null);
                    
                    foreach (var item in args.NewItems.Cast<TimingPoint>())
                    {
                        if (Children.Any(t => Math.Abs(t.Point.StartTime - item.StartTime) < 500 && t.IsRedundant(item)))
                            continue;
                        
                        Add(new DrawableTimingPoint(item));
                    }
                    break;
                
                case NotifyCollectionChangedAction.Remove:
                    Debug.Assert(args.OldItems != null);
                    
                    foreach (var item in args.OldItems.Cast<TimingPoint>())
                    {
                        var matching = Children.SingleOrDefault(t => ReferenceEquals(t.Point, item));
                        
                        if (matching != null)
                            matching.Expire();
                        else
                        {
                            Clear();

                            foreach (var p in timingPoints)
                                Add(new DrawableTimingPoint(p));
                        }
                    }
                    
                    break;
            }
        }, true);
    }
}
