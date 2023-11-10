namespace Kumi.Game.Gameplay.Algorithms;

public class LinearScrollAlgorithm : IScrollAlgorithm
{
    public double GetDrawableStartTime(double originTime, float offset, double timeRange, float length)
    {
        var time = TimeAt(-offset, originTime, timeRange, length);
        return time - timeRange;
    }

    public float PositionAt(double time, double currentTime, double timeRange, float length)
        => (float) ((time - currentTime) / timeRange * length);

    public double TimeAt(float position, double currentTime, double timeRange, float length)
        => position * timeRange / length + currentTime;
}
