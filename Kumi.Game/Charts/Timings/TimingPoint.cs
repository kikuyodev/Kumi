using Kumi.Game.IO;
using osu.Framework.Bindables;

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

    public TimingPointType PointType
    {
        get => PointTypeBindable.Value;
        set => PointTypeBindable.Value = value;
    }

    public TimingFlags Flags
    {
        get => FlagsBindable.Value;
        set => FlagsBindable.Value = value;
    }

    public int Volume
    {
        get => VolumeBindable.Value;
        set => VolumeBindable.Value = value;
    }

    public float RelativeScrollSpeed
    {
        get => RelativeScrollSpeedBindable.Value;
        set => RelativeScrollSpeedBindable.Value = value;
    }

    public Bindable<TimingPointType> PointTypeBindable { get; set; } = new Bindable<TimingPointType>();
    public Bindable<TimingFlags> FlagsBindable { get; set; } = new Bindable<TimingFlags>();
    public Bindable<int> VolumeBindable { get; set; } = new Bindable<int>();
    public Bindable<float> RelativeScrollSpeedBindable { get; set; } = new Bindable<float>();

    public double StartTime
    {
        get => StartTimeBindable.Value;
        set => StartTimeBindable.Value = value;
    }
    
    public Bindable<double> StartTimeBindable { get; } = new Bindable<double>();

    internal TimingPoint(double time)
    {
        StartTime = time;
    }

    public static readonly TimingPoint DEFAULT = new DefaultTimingPoint(0);

    public int CompareTo(TimingPoint? other) => StartTime < other!.StartTime ? -1 : StartTime > other.StartTime ? 1 : 0;

    public bool Equals(TimingPoint? other)
        => StartTime == other?.StartTime;

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
