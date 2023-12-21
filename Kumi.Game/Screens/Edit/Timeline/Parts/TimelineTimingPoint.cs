using Kumi.Game.Charts.Timings;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class TimelineTimingPoint : CompositeDrawable
{
    public readonly TimingPoint Point;

    public TimelineTimingPoint(TimingPoint point)
    {
        Point = point;

        RelativePositionAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;

        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomCentre;

        X = (float) point.StartTime;
        
        AddInternal(new TimingPointPiece(Point));
    }
}
