using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Timings;

/// <summary>
/// A class that handles everything related to timing points in a <see cref="IChart"/>.
/// </summary>
public class TimingPointHandler
{
    /// <summary>
    /// A list of all timing points in the <see cref="IChart"/>.
    /// </summary>
    public IBindableList<TimingPoint> TimingPoints { get; } = new BindableList<TimingPoint>();

    /// <summary>
    /// Gets any timing point at a given time.
    /// </summary>
    /// <param name="time">The time.</param>
    public TimingPoint GetTimingPointAt(float time) => searchForPoint<UninheritedTimingPoint>(time, null);
    
    /// <summary>
    /// Gets a specific kind of timing point at a given time.
    /// </summary>
    /// <param name="time"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetTimingPointAt<T>(float time, TimingPointType? pointType)
        where T : TimingPoint
        => searchForPoint<T>(time, pointType);
    
    /// <summary>
    /// Gets the scroll speed at a given time.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public float GetScrollSpeedAt(float time) => searchForPoint<TimingPoint>(time, null).RelativeScrollSpeed;
    
    private T searchForPoint<T>(float time, TimingPointType? pointType)
        where T : TimingPoint
    {
        int l = 0;
        int r = TimingPoints.Count - 1;
        
        while (l <= r)
        {
            int m = l + (r - l) / 2;

            if (TimingPoints[m].StartTime <= time && time < TimingPoints[m + 1].StartTime)
            {
                if (pointType == null || TimingPoints[m].PointType == pointType)
                    return TimingPoints[m] as T;
                
                // There's no point in continuing the search if that condition isn't met.
                return TimingPoint.DEFAULT as T;
            }
            
            if (TimingPoints[m].StartTime < time)
                l = m + 1;
            else
                r = m - 1;
        }
        
        return TimingPoint.DEFAULT as T;
    }
}
