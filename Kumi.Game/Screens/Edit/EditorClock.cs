using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace Kumi.Game.Screens.Edit;

public partial class EditorClock : CompositeComponent, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    private bool playbackFinished;
    
    private readonly DecouplingFramedClock underlyingClock = new DecouplingFramedClock { AllowDecoupling = true };
    private readonly Bindable<Track> track = new Bindable<Track>();

    public IBindable<Track> Track => track;
    public double TrackLength => track.Value?.IsLoaded == true ? track.Value.Length : 60000;

    public double CurrentTime => underlyingClock.CurrentTime;

    public void Reset()
    {
        underlyingClock.Reset();
    }

    public void Start()
    {
        if (playbackFinished)
            underlyingClock.Seek(0);

        underlyingClock.Start();
    }

    public void Stop()
    {
        underlyingClock.Stop();
    }

    public bool Seek(double position)
    {
        position = Math.Clamp(position, 0, TrackLength);
        return underlyingClock.Seek(position);
    }

    public void SeekForward(double amount = 1000)
        => seek(1, amount);

    public void SeekBackward(double amount = 1000)
        => seek(-1, amount);

    private void seek(int direction, double amount = 1000)
    {
        if (direction == 0)
            return;

        Seek(CurrentTime + (amount * direction));
    }

    public void ResetSpeedAdjustments()
        => underlyingClock.ResetSpeedAdjustments();

    double IAdjustableClock.Rate
    {
        get => underlyingClock.Rate;
        set => underlyingClock.Rate = value;
    }

    double IClock.Rate => underlyingClock.Rate;

    public bool IsRunning => underlyingClock.IsRunning;

    public void ProcessFrame()
    {
        underlyingClock.ProcessFrame();
    }

    public double ElapsedFrameTime => underlyingClock.ElapsedFrameTime;

    public double FramesPerSecond => underlyingClock.FramesPerSecond;

    public void ChangeSource(IClock source)
    {
        track.Value = (source as Track)!;
        underlyingClock.ChangeSource(source);
    }

    public IClock? Source => underlyingClock.Source;

    protected override void Update()
    {
        base.Update();

        playbackFinished = CurrentTime >= TrackLength;

        if (!playbackFinished)
            return;

        if (IsRunning)
            underlyingClock.Stop();

        if (CurrentTime > TrackLength)
            underlyingClock.Seek(TrackLength);
    }
}
