using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Objects;

public class Balloon : Note, IHasEndTime
{
    public float EndTime
    {
        get => endTimeBindable.Value;
        set => endTimeBindable.Value = value;
    }

    public Balloon(float startTime)
        : base(startTime)
    {
    }

    private readonly Bindable<float> endTimeBindable = new Bindable<float>();
}
