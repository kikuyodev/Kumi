using Kumi.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// A media event that can be used to switch the background or video in the <see cref="IChart"/>.
/// </summary>
public class SwitchMediaEvent : MediaEvent, IHasEndTime
{
    protected override EventType ExpectedType => EventType.SwitchMedia;
    protected override int ExpectedLength => 3;

    public float EndTime { get; private set; }
    public IEasingFunction Easing { get; private set; } = null!;

    public SwitchMediaEvent(string filename, float startTime, float endTime, Easing easing = osu.Framework.Graphics.Easing.None)
        : this(filename, startTime, endTime, new DefaultEasingFunction(easing))
    {
    }

    public SwitchMediaEvent(string filename, float startTime, float endTime, IEasingFunction easing)
        : base(filename, startTime)
    {
        EndTime = endTime;
        Easing = easing;
    }

    internal SwitchMediaEvent()
        : base(-1)
    {
        
    }

    protected override void Parse(string[] input)
    {
        EndTime = StartTime + StringUtils.AssertAndFetch<float>(input[0]);
        FileName = input[1];
        Easing = new DefaultEasingFunction(Enum.Parse<Easing>(input[2]));
    }
}
