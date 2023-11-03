using osu.Framework.Platform;

namespace Kumi.Game.Charts.Events;

/// <summary>
/// An event that uses a media file (video or image) in the <see cref="IChart"/>.
/// </summary>
public abstract class MediaEvent : Event, IHasMedia
{
    public string FileName { get; protected set; } = null!;

    protected MediaEvent(string filename, float time)
        : base(time)
    {
        StartTime = time;
        FileName = filename;
    }
    
    protected MediaEvent(float time)
        : base(time)
    {
        StartTime = time;
    }

    public bool FileExists(Storage chartStorage)
        => chartStorage.Exists(FileName);
    
    public Stream? GetStream(Storage chartStorage)
        => chartStorage.GetStream(FileName);

    public bool IsVideoFile()
    {
        // todo
        return false;
    }
    
    public bool IsImageFile()
    {
        // todo
        return false;
    }
}
