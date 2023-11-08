using Kumi.Game.IO.Formats;

namespace Kumi.Game.Charts.Events;

public interface IEvent : IHasTime, ICanParse<string>
{
    /// <summary>
    /// The type of this <see cref="IEvent"/>.
    /// </summary>
    EventType Type { get; }
}

public enum EventType
{
    Unknown = -1,
    SetMedia = 0,
    SwitchMedia = 1,
    Break = 2,
    KiaiTime = 3,
    DisplayLyric = 4
}
