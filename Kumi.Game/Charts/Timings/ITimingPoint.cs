namespace Kumi.Game.Charts.Timings;

public interface ITimingPoint : IHasTime
{   
    /// <summary>
    /// The type of timing point this is.
    /// </summary>
    TimingPointType PointType { get; }

    /// <summary>
    /// The timing flags of this timing point.
    /// </summary>
    TimingFlags Flags { get; }

    /// <summary>
    /// The volume of this timing point.
    /// </summary>
    int Volume { get; }
    
    /// <summary>
    /// Gets the relative scroll speed of this timing point.
    ///
    /// This can be used to calculate the scroll speed of a <see cref="IHasTime"/> object.
    /// </summary>
    float RelativeScrollSpeed { get; }
}

public enum TimingPointType
{
    Uninherited,
    Inherited
}

[Flags]
public enum TimingFlags
{
    ResetBarline = 1 << 0,
    ShowBarline = 1 << 1,
    HideBarline = 1 << 2,
}