using Kumi.Game.Utils;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// An event that defines a break in the <see cref="IChart" />.
/// </summary>
public class BreakTimeEvent : Event, IHasEndTime
{
    protected override EventType ExpectedType => EventType.Break;
    protected override int ExpectedLength => 1;

    public double EndTime { get; private set; }

    public BreakTimeEvent(float startTime, float endTime)
        : base(startTime)
    {
        Time = startTime;
        EndTime = endTime;
    }

    internal BreakTimeEvent()
        : base(-1)
    {
    }

    protected override void Parse(string[] input)
    {
        EndTime = Time + StringUtils.AssertAndFetch<float>(input[0]);
    }
}
