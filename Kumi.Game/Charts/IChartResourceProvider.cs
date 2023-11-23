using Kumi.Game.IO;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Charts;

public interface IChartResourceProvider : IStorageResourceProvider
{
    /// <summary>
    /// Retrieve a global large texture store, used for loading chart backgrounds.
    /// </summary>
    TextureStore LargeTextureStore { get; }

    /// <summary>
    /// Access a global track store for retrieving chart tracks from.
    /// </summary>
    ITrackStore Tracks { get; }
}
