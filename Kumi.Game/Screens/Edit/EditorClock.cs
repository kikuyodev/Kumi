using Kumi.Game.Charts;
using Kumi.Game.Charts.Timings;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;
using osu.Framework.Utils;

namespace Kumi.Game.Screens.Edit;

public partial class EditorClock : CompositeComponent, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    private bool playbackFinished;
    
    private readonly DecouplingFramedClock underlyingClock = new DecouplingFramedClock { AllowDecoupling = true };
    private readonly Bindable<Track> track = new Bindable<Track>();
    private readonly BindableBeatDivisor beatDivisor;

    public IBindable<Track> Track => track;
    public double TrackLength => track.Value?.IsLoaded == true ? track.Value.Length : 60000;

    public double CurrentTime => underlyingClock.CurrentTime;

    [Resolved]
    private Bindable<WorkingChart> chart { get; set; } = null!;

    public EditorClock(BindableBeatDivisor? beatDivisor = null)
    {
        this.beatDivisor = beatDivisor ?? new BindableBeatDivisor();
    }

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

    public void SeekForward(double amount = 1)
        => seek(1, amount);

    public void SeekBackward(double amount = 1)
        => seek(-1, amount);

    private void seek(int direction, double amount = 1)
    {
        if (direction == 0)
            return;

        var nativeChart = (Chart)chart.Value.Chart;
        var point = nativeChart.TimingHandler.GetTimingPointAt<UninheritedTimingPoint>(CurrentTime, TimingPointType.Uninherited);

        if (direction < 0 && point.Time == CurrentTime)
            point = nativeChart.TimingHandler.GetTimingPointAt<UninheritedTimingPoint>(CurrentTime - 1, TimingPointType.Uninherited);
        
        var seekAmount = point.MillisecondsPerBeat / beatDivisor.Value * amount;
        var newPosition = CurrentTime + (seekAmount * direction);

        if (nativeChart.TimingHandler.UninheritedTimingPoints.Count == 0)
        {
            Seek(newPosition);
            return;
        }

        newPosition -= point.Time;

        int closestBeat;
        if (direction > 0)
            closestBeat = (int) Math.Floor(newPosition / seekAmount);
        else
            closestBeat = (int) Math.Ceiling(newPosition / seekAmount);

        newPosition = point.Time + closestBeat * seekAmount;

        var nextPoint = nativeChart.TimingHandler.UninheritedTimingPoints.FirstOrDefault(t => t.Time > point.Time);
        if (newPosition > nextPoint?.Time)
            newPosition = nextPoint.Time;

        if (Precision.AlmostEquals(CurrentTime, newPosition, 0.5f))
        {
            closestBeat += direction > 0 ? 1 : -1;
            newPosition = point.Time + closestBeat * seekAmount;
        }
        
        if (newPosition < point.Time && !ReferenceEquals(point, nativeChart.TimingHandler.UninheritedTimingPoints.First()))
            newPosition = point.Time;
        
        Seek(newPosition);
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
