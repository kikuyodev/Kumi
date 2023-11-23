namespace Kumi.Game.Charts.Objects.Windows;

public class NoteWindowRange
{
    public readonly NoteHitResult Result;
    public float Min { get; }
    public float Max { get; }

    public NoteWindowRange(NoteHitResult result, float value)
    {
        Result = result;
        Min = -value;
        Max = value;
    }
}
