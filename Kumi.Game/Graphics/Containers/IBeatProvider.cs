using Kumi.Game.Charts.Timings;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Timing;

namespace Kumi.Game.Graphics.Containers;

[Cached]
public interface IBeatProvider : IHasAmplitudes
{
    TimingPointHandler TimingPointHandler { get; }
    
    IClock Clock { get; }
}
