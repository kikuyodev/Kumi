namespace Kumi.Game.Gameplay.Algorithms;

public interface IScrollAlgorithm
{
    double GetDrawableStartTime(double originTime, float offset, double timeRange, float length);

    float PositionAt(double time, double currentTime, double timeRange, float length);

    double TimeAt(float position, double currentTime, double timeRange, float length);
}
