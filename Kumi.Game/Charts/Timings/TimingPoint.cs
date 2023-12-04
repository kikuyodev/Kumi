using Kumi.Game.IO;

namespace Kumi.Game.Charts.Timings;

/// <summary>
/// A point in time that affects the chart in some way, i.e. BPM changes, scroll speed changes, etc.
/// </summary>
public class TimingPoint : ITimingPoint, IComparablyEquatable<TimingPoint>
{
    /// <summary>
    /// The delimiter used to split the input string.
    /// </summary>
    public const char DELIMITER = ',';

    public TimingPointType PointType { get; set; }
    public TimingFlags Flags { get; set; }
    public int Volume { get; set; }
    public float RelativeScrollSpeed { get; set; }
    public double Time { get; }

    internal TimingPoint(double time)
    {
        Time = time;
    }

    public static readonly TimingPoint DEFAULT = new DefaultTimingPoint(0);

    public int CompareTo(TimingPoint? other) => Time < other!.Time ? -1 : Time > other.Time ? 1 : 0;

    public bool Equals(TimingPoint? other)
    {
        if (ReferenceEquals(other, null))
            return false;

        if (ReferenceEquals(other, this))
            return false;

        return Time.Equals(other.Time);
    }

    internal class DefaultTimingPoint : UninheritedTimingPoint
    {
        public DefaultTimingPoint(float time)
            : base(time)
        {
            BPM = 120;
            TimeSignature = new TimeSignature(4, 4);
            Volume = 100;
            RelativeScrollSpeed = 1.0f;
        }
    }
}
