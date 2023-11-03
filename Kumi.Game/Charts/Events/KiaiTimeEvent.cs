using Kumi.Game.Utils;

namespace Kumi.Game.Charts.Data.Events;

/// <summary>
/// An event that defines a break in the <see cref="IChart"/>.
///
/// This is based off the Go-Go time event typically found in
/// Taiko no Tatsujin games.
/// </summary>
public class KiaiTimeEvent : Event, IHasEndTime
{
    protected override EventType ExpectedType => EventType.KiaiTime;
    protected override int ExpectedLength => 1;

    public float StartTime { get; }
    public float EndTime { get; private set; }

    public KiaiTimeEvent(float startTime, float endTime)
        : base(startTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
    protected override void Parse(string[] input)
    {
        EndTime = StartTime + StringUtils.AssertAndFetch<float>(input[0]);
    }
}
