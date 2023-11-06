using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace Kumi.Game.Gameplay.Clocks;

public interface IGameplayClock : IFrameBasedClock
{
    double StartTime { get; }
    
    IBindable<bool> IsPaused { get; }
}
