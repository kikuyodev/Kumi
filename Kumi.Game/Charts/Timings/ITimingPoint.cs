using Kumi.Game.Charts.Data;

namespace Kumi.Game.Charts.Timings;

public interface ITimingPoint : IHasTime
{   
    /// <summary>
    /// The type of timing point this is.
    /// </summary>
    public TimingPointType PointType { get; }

    /// <summary>
    /// The timing flags of this timing point.
    /// </summary>
    public TimingFlags Flags { get; }

    /// <summary>
    /// The volume of this timing point.
    /// </summary>
    public int Volume { get; }
    
    /// <summary>
    /// Gets the relative scroll speed of this timing point.
    ///
    /// This can be used to calculate the scroll speed of a <see cref="IHasTime"/> object.
    /// </summary>
    public float RelativeScrollSpeed { get; }
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