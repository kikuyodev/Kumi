using Kumi.Game.Utils;
using osu.Framework.Graphics;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// A media event that can be used to switch the background or video in the <see cref="IChart" />.
/// </summary>
public class SwitchMediaEvent : MediaEvent, IHasEndTime
{
    protected override EventType ExpectedType => EventType.SwitchMedia;
    protected override int ExpectedLength => 3;

    public double EndTime { get; private set; }
    public Easing Easing { get; private set; }

    public SwitchMediaEvent(string filename, float startTime, float endTime, Easing easing = Easing.None)
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
        EndTime = StartTime + StringUtils.AssertAndFetch<double>(input[0]);
        FileName = input[1];
        Easing = Enum.Parse<Easing>(input[2]);
    }
}
