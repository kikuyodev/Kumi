using Kumi.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;

namespace Kumi.Game.Charts.Data.Events;

/// <summary>
/// An event that displays a lyric on the screen during a certain time.
/// </summary>
public class DisplayLyricEvent : Event, IHasEndTime
{
    protected override EventType ExpectedType => EventType.SwitchMedia;
    protected override int ExpectedLength => 4;

    public float StartTime { get; }
    public float EndTime { get; private set; }
    public float CrossfadeTime { get; private set; }
    public IEasingFunction Easing { get; private set; }
    public string Lyric { get; private set; }

    public DisplayLyricEvent(string lyric, float startTime, float endTime, float crossfadeTime, Easing easing = osu.Framework.Graphics.Easing.None)
        : this(lyric, startTime, endTime, crossfadeTime, new DefaultEasingFunction(easing))
    {
    }

    public DisplayLyricEvent(string lyric, float startTime, float endTime, float crossfadeTime, IEasingFunction easing)
        : base(startTime)
    {
        Lyric = lyric;
        StartTime = startTime;
        EndTime = endTime;
        CrossfadeTime = crossfadeTime;
        Easing = easing;
    }
    
    protected override void Parse(string[] input)
    {
        EndTime = StartTime + StringUtils.AssertAndFetch<float>(input[0]);
        CrossfadeTime = StringUtils.AssertAndFetch<float>(input[1]);
        Easing = new DefaultEasingFunction(Enum.Parse<Easing>(input[2]));
        Lyric = input[3];
    }
}
