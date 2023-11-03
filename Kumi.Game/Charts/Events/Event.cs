namespace Kumi.Game.Charts.Data;

/// <summary>
/// An event that occurs at a specific time.
/// </summary>
public abstract class Event : IEvent
{
    public float StartTime { get; set; }
    public EventType Type { get; set; }
    
    /// <summary>
    /// The delimiter used to split the input string.
    /// </summary>
    protected virtual string Delimiter => ",";
    
    /// <summary>
    /// The expected length of the input string once it has been split, excluding the <see cref="EventType"/> and <see cref="StartTime"/>.
    /// </summary>
    protected virtual int ExpectedLength => 0;

    /// <summary>
    /// The expected <see cref="EventType"/> of the event.
    /// </summary>
    protected virtual EventType ExpectedType => EventType.Unknown;
    
    public Event(float startTime)
    {
        StartTime = startTime;
    }

    public void ParseFrom(string input)
    {
        var split = input.Split(Delimiter);
        
        if (split.Length != ExpectedLength + 2)
            throw new ArgumentException($"Expected {ExpectedLength + 2} values for the event {nameof(Event)}, but got {split.Length}.");
        
        if (!Enum.TryParse(split[0], out EventType type))
            throw new ArgumentException($"Could not parse the event type from the input string: {split[0]}.");
        
        if (type != ExpectedType)
            throw new ArgumentException($"Expected the event type to be {ExpectedType}, but got {type}.");
        
        Type = type;
        StartTime= float.Parse(split[1]);
        
        Parse(split[2..]);
    }
    
    /// <summary>
    /// An internal method that is called by <see cref="ParseFrom"/> to parse the input string.
    ///
    /// The input string is guaranteed to be split by <see cref="Delimiter"/> and have a length of <see cref="ExpectedLength"/>.
    /// The first two values are the <see cref="EventType"/> and the <see cref="StartTime"/> respectively; and are already parsed;
    /// and the remaining values passed to this function are the values that need to be parsed.
    /// </summary>
    /// <param name="input">The input remaining to be parsed.</param>
    protected abstract void Parse(string[] input);
}