using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Threading;

namespace Kumi.Game.Audio;

[LongRunningLoad]
public abstract partial class TrackPreview : Component
{
    public event Action? Stopped;
    public event Action? Started;

    protected Track? Track { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Track = GetTrack();

        if (Track != null)
            Track.Completed += Stop;
    }

    private ScheduledDelegate? startDelegate;

    private bool hasStarted;

    public bool Start()
    {
        if (Track == null)
            return false;

        startDelegate = Schedule(() =>
        {
            if (hasStarted)
                return;

            hasStarted = true;

            Track.Restart();
            Started?.Invoke();
        });

        return true;
    }

    public void Stop()
    {
        startDelegate?.Cancel();

        if (Track == null || !hasStarted)
            return;

        hasStarted = false;

        if (!Track.HasCompleted)
        {
            Track.Stop();
            Track.Seek(0);
        }

        Stopped?.Invoke();
    }

    protected abstract Track? GetTrack();

    #region Re-expose Track properties

    public double Length => Track?.Length ?? 0;

    public double CurrentTime => Track?.CurrentTime ?? 0;

    public bool TrackLoaded => Track?.IsLoaded ?? false;

    public bool IsRunning => Track?.IsRunning ?? false;

    #endregion

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Stop();
        Track?.Dispose();
    }
}
