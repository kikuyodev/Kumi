namespace Kumi.Game.Charts.Data;

public interface IHasEndTime
{
    /// <summary>
    /// The time in milliseconds at which this <see cref="IHasEndTime"/> ends.
    /// </summary>
    float EndTime { get; }
}
