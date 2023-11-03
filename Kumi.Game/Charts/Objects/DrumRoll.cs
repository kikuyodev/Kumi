using Kumi.Game.Charts;
using osu.Framework.Bindables;
using osuTK.Graphics;

namespace Kumi.Game.Charts.Objects;

/// <summary>
/// A drum roll note, with a start and end time.
/// </summary>
public class DrumRoll : Note, IHasEndTime
{
    public float EndTime
    {
        get => _endTimeBindable.Value;
        set => _endTimeBindable.Value = value;
    }
    
    private Bindable<float> _endTimeBindable = new();
}
