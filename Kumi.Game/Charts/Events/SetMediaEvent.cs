using osu.Framework.Platform;

namespace Kumi.Game.Charts.Data.Events;

/// <summary>
/// Sets the initial background or video in the <see cref="IChart"/>.
/// </summary>
public class SetMediaEvent : MediaEvent
{
    protected override EventType ExpectedType => EventType.SetMedia;
    protected override int ExpectedLength => 1;
    
    public SetMediaEvent(string filename, float time)
        : base(filename, time)
    {
    }

    protected override void Parse(string[] input)
    {
        FileName = input[0];
    }
}
