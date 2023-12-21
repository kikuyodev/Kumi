using Kumi.Game.Charts.Timings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingPointFlowList : FillFlowContainer
{
    [Resolved(name: "selected_point")]
    private Bindable<TimingPoint> selectedPoint { get; set; } = null!;

    public IEnumerable<TimingPoint> TimingPoints
    {
        set
        {
            Clear();
            
            if (!value.Any())
                return;

            foreach (var point in value)
                Add(new DrawableTimingPoint(point));
        }
    }

    public TimingPointFlowList()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 2);

        AutoSizeDuration = 200;
        AutoSizeEasing = Easing.OutQuint;
    }
}
