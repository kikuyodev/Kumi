using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Timings;

/// <summary>
/// A timing point that doesn't inherit it's properties from previous timing points.
/// </summary>
public class UninheritedTimingPoint : TimingPoint
{
    /// <summary>
    /// The BPM of this timing point.
    /// </summary>
    public float BPM
    {
        get => _bpmBindable.Value;
        set => _bpmBindable.Value = value;
    }

    /// <summary>
    /// The time signature of this timing point.
    /// </summary>
    public TimeSignature TimeSignature { get; set; } = TimeSignature.COMMON_TIME;
    
    public UninheritedTimingPoint(float time)
        : base(time)
    {
    }
    
    /// <summary>
    /// The milliseconds necessary to elapse a beat for this timing point.
    /// </summary>
    public float MillisecondsPerBeat => (60000f / BPM) * TimeSignature.NoteSubdivision;
    
    public Bindable<float> GetBindableBPM() => _bpmBindable;

    private Bindable<float> _bpmBindable = new();
}
