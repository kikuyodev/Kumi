using osu.Framework.Allocation;
using osu.Framework.Bindables;

namespace Kumi.Game.Audio;

[Cached]
public interface ISamplePausablePlayback
{
    IBindable<bool> SamplePaused { get; }
}
