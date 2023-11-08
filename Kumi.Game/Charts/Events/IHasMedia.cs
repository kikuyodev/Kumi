using osu.Framework.Platform;

namespace Kumi.Game.Charts.Events;

public interface IHasMedia
{
    /// <summary>
    /// The file name of the media.
    /// </summary>
    string FileName { get; }

    bool FileExists(Storage chartStorage);
    Stream? GetStream(Storage chartStorage);

    bool IsVideoFile();
    bool IsImageFile();
}
