namespace Kumi.Game.Charts.Objects.Windows;

public class NoteWindows
{
    /// <summary>
    /// The standard note windows for difficulty ratings between 1.0 and 6.0.
    /// </summary>
    public static readonly NoteWindowRange[] LOWER_NOTE_WINDOWS =
    {
        new NoteWindowRange(NoteHitResult.Bad, 125.0f),
        new NoteWindowRange(NoteHitResult.Ok, 108.0f),
        new NoteWindowRange(NoteHitResult.Good, 42.0f)
    };

    /// <summary>
    /// The standard note windows for difficulty ratings between 6.0 and 10.0.
    /// </summary>
    public static readonly NoteWindowRange[] UPPER_NOTE_WINDOWS =
    {
        new NoteWindowRange(NoteHitResult.Bad, 125.0f),
        new NoteWindowRange(NoteHitResult.Ok, 108.0f),
        new NoteWindowRange(NoteHitResult.Good, 42.0f)
    };

    /// <summary>
    /// The final difficulty rating used to calculate these note windows.
    /// </summary>
    public float DifficultyRating { get; set; }

    /// <summary>
    /// The note windows for this difficulty rating.
    /// </summary>
    public NoteWindowRange[] WindowRanges => DifficultyRating <= 6.0f ? LOWER_NOTE_WINDOWS : UPPER_NOTE_WINDOWS;

    /// <summary>
    /// Applies a difficulty rating to this <see cref="NoteWindows" /> instance.
    /// </summary>
    /// <param name="rating"></param>
    public void ApplyDifficultyRating(float rating)
    {
        DifficultyRating = rating;
    }

    /// <summary>
    /// Gets the <see cref="NoteHitResult" /> for a given delta.
    /// </summary>
    /// <param name="delta">The time this note was hit.</param>
    public NoteHitResult? Result(double delta)
    {
        if (!IsWithinWindow(delta))
            return null;

        if (delta < WindowRanges[0].Min)
            return NoteHitResult.Bad;
        if (delta < WindowRanges[0].Max)
            return NoteHitResult.Ok;
        if (delta < WindowRanges[1].Max)
            return NoteHitResult.Good;

        return NoteHitResult.Bad;
    }

    /// <summary>
    /// Determines whether a given delta is within the note windows.
    /// </summary>
    /// <param name="delta">The time this note was hit.</param>
    public bool IsWithinWindow(double delta) => delta >= WindowRanges[0].Min && delta <= WindowRanges[0].Max;

    public float WindowFor(NoteHitResult result)
    {
        return result switch
        {
            NoteHitResult.Good => WindowRanges[2].Max,
            NoteHitResult.Ok => WindowRanges[1].Max,
            NoteHitResult.Bad => WindowRanges[0].Max,
            _ => throw new ArgumentException("Invalid NoteHitResult", nameof(result))
        };
    }
}
