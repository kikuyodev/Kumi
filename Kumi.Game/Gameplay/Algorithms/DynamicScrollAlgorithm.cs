using Kumi.Game.Charts.Timings;

namespace Kumi.Game.Gameplay.Algorithms;

public class DynamicScrollAlgorithm : IScrollAlgorithm
{
    private readonly IReadOnlyList<TimingPoint> timingPoints;

    private readonly List<PositionMapping> positionMappings = new List<PositionMapping>();
    
    public DynamicScrollAlgorithm(IReadOnlyList<TimingPoint> timingPoints)
    {
        this.timingPoints = timingPoints;
    }

    public double GetDrawableStartTime(double originTime, float offset, double timeRange, float length)
    {
        return TimeAt(-(length + offset), originTime, timeRange, length);
    }

    public float GetLength(double startTime, double endTime, double timeRange, float length)
    {
        var noteLength = relativePositionAt(endTime, timeRange) - relativePositionAt(startTime, timeRange);
        return (float) (noteLength * length);
    }

    public float PositionAt(double time, double currentTime, double timeRange, float length)
    {
        var timelineLength = relativePositionAt(time, timeRange) - relativePositionAt(currentTime, timeRange);
        return (float) (timelineLength * length);
    }

    public double TimeAt(float position, double currentTime, double timeRange, float length)
    {
        if (timingPoints.Count == 0)
            return position * timeRange;

        var relativePosition = relativePositionAt(currentTime, timeRange) + position / length;
        var positionMapping = findTimingPointMapping(timeRange, new PositionMapping(0, null, relativePosition),
            Comparer<PositionMapping>.Create((c1, c2) => c1.Position.CompareTo(c2.Position)));

        return positionMapping.Time + (relativePosition - positionMapping.Position) * timeRange / positionMapping.TimingPoint!.RelativeScrollSpeed;
    }

    private double relativePositionAt(in double time, in double timeRange)
    {
        if (timingPoints.Count == 0)
            return time / timeRange;

        var mapping = findTimingPointMapping(timeRange, new PositionMapping(time));

        return mapping.Position + (time - mapping.Time) / timeRange * mapping.TimingPoint!.RelativeScrollSpeed;
    }
    
    private PositionMapping findTimingPointMapping(in double timeRange, in PositionMapping search, IComparer<PositionMapping>? comparer = null)
    {
        generatePositionMappings(timeRange);
        
        var mappingIndex = positionMappings.BinarySearch(search, comparer ?? Comparer<PositionMapping>.Default);

        if (mappingIndex < 0)
        {
            mappingIndex = Math.Max(0, ~mappingIndex - 1);
        }

        var mapping = positionMappings[mappingIndex];
        return mapping;
    }

    private void generatePositionMappings(in double timeRange)
    {
        if (positionMappings.Count > 0)
            return;
        
        if (timingPoints.Count == 0)
            return;
        
        positionMappings.Add(new PositionMapping(timingPoints[0].StartTime, timingPoints[0]));

        for (var i = 0; i < timingPoints.Count - 1; i++)
        {
            var current = timingPoints[i];
            var next = timingPoints[i + 1];

            var length = (float) ((next.StartTime - current.StartTime) / timeRange * current.RelativeScrollSpeed);
            
            positionMappings.Add(new PositionMapping(next.StartTime, next, positionMappings[^1].Position + length));
        }
    }
    
    private readonly struct PositionMapping : IComparable<PositionMapping>
    {
        public readonly double Time;
        public readonly double Position;
        public readonly TimingPoint? TimingPoint;

        public PositionMapping(double time, TimingPoint? timingPoint = null, double position = default)
        {
            Time = time;
            Position = position;
            TimingPoint = timingPoint;
        }

        public int CompareTo(PositionMapping other) => Time.CompareTo(other.Time);
    }
}
