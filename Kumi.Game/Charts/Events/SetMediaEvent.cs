using osu.Framework.Platform;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// Sets the initial background or video in the <see cref="IChart"/>.
/// </summary>
public class SetMediaEvent : MediaEvent
{
    protected override EventType ExpectedType => EventType.SetMedia;
    protected override int ExpectedLength => 1;
    
    public SetMediaEvent(string filename)
        : base(filename, -1)
    {
    }

    internal SetMediaEvent()
        : base(-1)
    {
    }

    protected override void Parse(string[] input)
    {
        FileName = input[0];
    }
}
