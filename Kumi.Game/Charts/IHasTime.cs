namespace Kumi.Game.Charts;

public interface IHasTime
{
    /// <summary>
    /// The time in milliseconds at which this <see cref="IHasTime" /> starts.
    /// </summary>
    double Time { get; }
}
