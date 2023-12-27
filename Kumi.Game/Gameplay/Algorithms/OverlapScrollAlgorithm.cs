using Kumi.Game.Charts.Timings;

namespace Kumi.Game.Gameplay.Algorithms;

public class OverlapScrollAlgorithm : IScrollAlgorithm
{
    private readonly TimingPointHandler handler;

    public OverlapScrollAlgorithm(TimingPointHandler handler)
    {
        this.handler = handler;
    }

    public double GetDrawableStartTime(double originTime, float offset, double timeRange, float length)
    {
        var point = handler.GetTimingPointAt(originTime);

        var visibleDuration = (length + offset) * timeRange / point.RelativeScrollSpeed / length;
        return originTime - visibleDuration;
    }

    public float GetLength(double startTime, double endTime, double timeRange, float length)
        => -PositionAt(startTime, endTime, timeRange, length);

    public float PositionAt(double time, double currentTime, double timeRange, float length)
        => (float) ((time - currentTime) / timeRange * handler.GetTimingPointAt(time).RelativeScrollSpeed * length);

    public double TimeAt(float position, double currentTime, double timeRange, float length)
    {
        var relevantPoint = handler.TimingPoints.LastOrDefault(p => PositionAt(p.StartTime, currentTime, timeRange, length) <= position) ?? handler.TimingPoints.First();
        var positionAtPoint = PositionAt(relevantPoint.StartTime, currentTime, timeRange, length);
        
        return relevantPoint.StartTime + (position - positionAtPoint) * timeRange / relevantPoint.RelativeScrollSpeed / length;
    }
}
