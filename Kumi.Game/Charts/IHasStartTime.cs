namespace Kumi.Game.Charts;

public interface IHasStartTime
{
    /// <summary>
    /// The time in milliseconds at which this <see cref="IHasStartTime" /> starts.
    /// </summary>
    double StartTime { get; }
}
