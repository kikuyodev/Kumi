using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Objects;

public class Balloon : Note, IHasEndTime
{
    public double EndTime
    {
        get => EndTimeBindable.Value;
        set
        {
            if (EndTimeBindable.Value == value)
                return;

            if (value < StartTime)
                throw new ArgumentOutOfRangeException(nameof(value), "End time cannot be less than start time.");

            EndTimeBindable.Value = value;
        }
    }

    public Bindable<double> EndTimeBindable { get; } = new Bindable<double>();

    public Balloon(float startTime)
        : base(startTime)
    {
    }
}
