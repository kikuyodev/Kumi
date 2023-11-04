using Kumi.Game.Charts;
using osu.Framework.Bindables;

namespace Kumi.Game.Charts.Objects;

public class Balloon : Note, IHasEndTime
{
    public float EndTime
    {
        get => _endTimeBindable.Value;
        set => _endTimeBindable.Value = value;
    }

    public Balloon(float startTime)
        : base(startTime)
    {
    }
    
    private Bindable<float> _endTimeBindable = new();
}

