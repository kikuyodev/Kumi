using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Objects;

/// <summary>
/// A drum roll note, with a start and end time.
/// </summary>
public class DrumRoll : Note, IHasEndTime
{
    public double EndTime
    {
        get => endTimeBindable.Value;
        set => endTimeBindable.Value = value;
    }

    public DrumRoll(float startTime)
        : base(startTime)
    {
    }

    private readonly Bindable<double> endTimeBindable = new Bindable<double>();
}
