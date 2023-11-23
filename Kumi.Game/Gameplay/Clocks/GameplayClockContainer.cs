using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace Kumi.Game.Gameplay.Clocks;

public partial class GameplayClockContainer : Container, IAdjustableClock, IGameplayClock
{
    public IBindable<bool> IsPaused => isPaused;

    public double StartTime { get; set; }

    private readonly BindableBool isPaused = new BindableBool(true);
    private readonly DecouplingFramedClock clock;

    public GameplayClockContainer(IClock sourceClock)
    {
        RelativeSizeAxes = Axes.Both;

        Clock = clock = new DecouplingFramedClock(sourceClock);
        clock.Stop();
    }

    public void Start()
    {
        if (!isPaused.Value)
            return;

        isPaused.Value = false;

        SchedulerAfterChildren.Add(() =>
        {
            if (isPaused.Value)
                return;

            clock.Start();
        });
    }

    public bool Seek(double position) => clock.Seek(position);

    public void Stop()
    {
        if (isPaused.Value)
            return;

        isPaused.Value = true;
        clock.Stop();
    }

    public void Reset(double? time = null, bool startClock = false)
    {
        var wasPaused = isPaused.Value;

        clock.Stop();

        if (time != null)
            StartTime = time.Value;

        Seek(StartTime);

        if (!wasPaused || startClock)
            Start();
    }

    #region IAdjustableClock

    void IAdjustableClock.Reset() => Reset();

    public void ResetSpeedAdjustments()
    {
    }

    double IAdjustableClock.Rate
    {
        get => clock.Rate;
        set => throw new NotImplementedException();
    }

    public double Rate => clock.Rate;

    public double CurrentTime => clock.CurrentTime;

    public bool IsRunning => clock.IsRunning;

    #endregion

    public void ProcessFrame()
    {
    }

    public double ElapsedFrameTime => clock.ElapsedFrameTime;

    public double FramesPerSecond => clock.FramesPerSecond;
}
