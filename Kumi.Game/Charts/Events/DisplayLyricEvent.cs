using Kumi.Game.Utils;
using osu.Framework.Graphics;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// An event that displays a lyric on the screen during a certain time.
/// </summary>
public class DisplayLyricEvent : Event, IHasEndTime
{
    protected override EventType ExpectedType => EventType.SwitchMedia;
    protected override int ExpectedLength => 4;

    public float EndTime { get; private set; }
    public float CrossfadeTime { get; private set; }
    public Easing Easing { get; private set; }
    public string Lyric { get; private set; } = null!;

    public DisplayLyricEvent(string lyric, float startTime, float endTime, float crossfadeTime, Easing easing = Easing.None)
        : base(startTime)
    {
        Lyric = lyric;
        StartTime = startTime;
        EndTime = endTime;
        CrossfadeTime = crossfadeTime;
        Easing = easing;
    }

    internal DisplayLyricEvent()
        : base(-1)
    {
    }

    protected override void Parse(string[] input)
    {
        EndTime = StartTime + StringUtils.AssertAndFetch<float>(input[0]);
        Easing = Enum.Parse<Easing>(input[1]);
        CrossfadeTime = StringUtils.AssertAndFetch<float>(input[2]);
        Lyric = input[3];
    }
}
