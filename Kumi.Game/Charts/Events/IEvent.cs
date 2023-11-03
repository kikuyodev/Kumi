using Kumi.Game.IO.Formats;

namespace Kumi.Game.Charts.Data;

public interface IEvent : IHasTime, ICanParse<string>
{
    /// <summary>
    /// The type of this <see cref="IEvent"/>.
    /// </summary>
    EventType Type { get; }
}

public enum EventType : int
{
    Unknown = -1,
    SetMedia = 0,
    SwitchMedia = 1,
    Break = 2,
    Kiai = 3,
    DisplayLyric = 4
}
