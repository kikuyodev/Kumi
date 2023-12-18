using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class DrawableTimingPoint : DrawableTick
{
    public readonly TimingPoint Point;
    
    public DrawableTimingPoint(TimingPoint point)
        : base(point.StartTime)
    {
        Point = point;
        
        Height = 0.25f;
        Width = 2;
        Origin = point.PointType switch
        {
            TimingPointType.Uninherited => Anchor.BottomCentre,
            TimingPointType.Inherited => Anchor.TopCentre,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Colour = point.PointType switch
        {
            TimingPointType.Uninherited => Colours.RED_ACCENT_LIGHT,
            TimingPointType.Inherited => Colours.BLUE_ACCENT_LIGHT,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public bool IsRedundant(TimingPoint other)
        => other.GetType() == Point.GetType();
}
