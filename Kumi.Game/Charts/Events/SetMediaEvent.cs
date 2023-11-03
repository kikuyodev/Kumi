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
        : base(filename, 0)
    {
    }

    internal SetMediaEvent()
        : base(0)
    {
        
    }

    public override void ParseFrom(string[] input)
    {
        if (input.Length != ExpectedLength + 1)
            throw new ArgumentException($"Expected {ExpectedLength + 2} values for the event {nameof(Event)}, but got {input.Length}.");

        if (!Enum.TryParse(input[0], out EventType type))
            throw new ArgumentException($"Could not parse the event type from the input string: {input[0]}.");

        if (type != ExpectedType)
            throw new ArgumentException($"Expected the event type to be {ExpectedType}, but got {type}.");

        Type = type;
        FileName = input[1];
    }

    protected override void Parse(string[] input)
    {
    }
}
