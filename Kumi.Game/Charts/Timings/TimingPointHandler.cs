using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Timings;

/// <summary>
/// A class that handles everything related to timing points in a <see cref="IChart" />.
/// </summary>
public class TimingPointHandler
{
    /// <summary>
    /// A list of all timing points in the <see cref="IChart" />.
    /// </summary>
    public BindableList<TimingPoint> TimingPoints { get; } = new BindableList<TimingPoint>();
    
    /// <summary>
    /// A list of all inherited timing points in the <see cref="IChart" />.
    /// </summary>
    public IReadOnlyList<InheritedTimingPoint> InheritedTimingPoints => TimingPoints.OfType<InheritedTimingPoint>().ToList().AsReadOnly();
    
    /// <summary>
    /// A list of all uninherited timing points in the <see cref="IChart" />.
    /// </summary>
    public IReadOnlyList<UninheritedTimingPoint> UninheritedTimingPoints => TimingPoints.OfType<UninheritedTimingPoint>().ToList().AsReadOnly();

    /// <summary>
    /// Gets any timing point at a given time.
    /// </summary>
    /// <param name="time">The time.</param>
    public TimingPoint GetTimingPointAt(double time) => searchForPoint<TimingPoint>(time, null);

    /// <summary>
    /// Gets a specific kind of timing point at a given time.
    /// </summary>
    /// <param name="time">The time.</param>
    /// <param name="pointType">The point type.</param>
    public T GetTimingPointAt<T>(double time, TimingPointType? pointType)
        where T : TimingPoint
        => searchForPoint<T>(time, pointType);

    /// <summary>
    /// Gets the scroll speed at a given time.
    /// </summary>
    /// <param name="time">The time.</param>
    public float GetScrollSpeedAt(double time) => searchForPoint<TimingPoint>(time, null).RelativeScrollSpeed;

    /// <summary>
    /// Gets the BPM at a given time.
    /// </summary>
    /// <param name="time">The time.</param>
    public float GetBPMAt(double time) => searchForPoint<UninheritedTimingPoint>(time, TimingPointType.Uninherited).BPM;

    /// <summary>
    /// Gets the beat length at a given time.
    /// </summary>
    /// <param name="time">The time.</param>
    public float GetBeatLengthAt(double time) => searchForPoint<UninheritedTimingPoint>(time, TimingPointType.Uninherited).MillisecondsPerBeat;

    /// <summary>
    /// Clears all timing points.
    /// </summary>
    public void Clear()
    {
        TimingPoints.Clear();
    }

    private T searchForPoint<T>(double time, TimingPointType? pointType)
        where T : TimingPoint
    {
        if (TimingPoints.Count == 0)
            return (TimingPoint.DEFAULT as T)!;
        
        if (time < TimingPoints[0].StartTime)
            return (TimingPoint.DEFAULT as T)!;
        
        if (time > TimingPoints[^1].StartTime)
            return (TimingPoints[^1] as T)!;
        
        var idx = TimingPoints.BinarySearch(new TimingPoint(time));
        if (idx < 0)
            idx = ~idx - 1;

        return (TimingPoints[idx] is T point && (pointType == null || point.PointType == pointType) ? point : TimingPoint.DEFAULT as T)!;
    }
}
