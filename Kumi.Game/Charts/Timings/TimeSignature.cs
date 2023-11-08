namespace Kumi.Game.Charts.Timings;

/// <summary>
/// A representation of a time signature for a chart.
/// </summary>
public class TimeSignature
{
    /// <summary>
    /// The numerator of the time signature.
    /// </summary>
    public int Numerator { get; }
    
    /// <summary>
    /// The denominator of the time signature.
    /// </summary>
    public int Denominator { get; }
    
    public static readonly TimeSignature COMMON_TIME = new TimeSignature(4, 4);
    public static readonly TimeSignature WALTZ_TIME = new TimeSignature(3, 4);
    
    public TimeSignature(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }
    
    public float NoteSubdivision => (float)Numerator / Denominator;

    public override string ToString() => $"{Numerator}/{Denominator}";
}
