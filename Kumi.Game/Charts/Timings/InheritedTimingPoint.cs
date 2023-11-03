namespace Kumi.Game.Charts.Timings;

/// <summary>
/// A timing point that inherits it's properties from previous timing points.
/// </summary>
public class InheritedTimingPoint : TimingPoint
{
    public InheritedTimingPoint(float time)
        : base(time)
    {
    }
}
