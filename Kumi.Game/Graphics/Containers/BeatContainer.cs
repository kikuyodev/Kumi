using Kumi.Game.Charts.Timings;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Graphics.Containers;

public partial class BeatContainer : Container
{
    private int lastBeat;

    private TimingPoint? lastTimingPoint;

    public int Divisor { get; set; } = 1;

    protected bool IsBeatSynced { get; private set; }

    [Resolved]
    protected IBeatProvider BeatSource { get; private set; } = null!;

    protected virtual void OnBeat(int beatIndex, UninheritedTimingPoint uninheritedTimingPoint, TimingPoint timingPoint, ChannelAmplitudes amplitudes)
    {
    }

    protected override void Update()
    {
        UninheritedTimingPoint uninheritedPoint;
        TimingPoint closestPoint;

        IsBeatSynced = BeatSource.Clock.IsRunning;

        if (IsBeatSynced)
        {
            uninheritedPoint = BeatSource.TimingPointHandler.GetTimingPointAt<UninheritedTimingPoint>(BeatSource.Clock.CurrentTime, TimingPointType.Uninherited);
            closestPoint = BeatSource.TimingPointHandler.GetTimingPointAt(BeatSource.Clock.CurrentTime);
        }
        else
        {
            uninheritedPoint = (UninheritedTimingPoint) TimingPoint.DEFAULT;
            closestPoint = TimingPoint.DEFAULT;
        }

        var beatLength = uninheritedPoint.MillisecondsPerBeat / Divisor;
        var beatIndex = (int) ((BeatSource.Clock.CurrentTime - uninheritedPoint.StartTime) / beatLength);

        if (BeatSource.Clock.CurrentTime < uninheritedPoint.StartTime)
            beatIndex--;

        var timeUntilNextBeat = (uninheritedPoint.StartTime - BeatSource.Clock.CurrentTime) % beatLength;
        if (timeUntilNextBeat <= 0)
            timeUntilNextBeat += beatLength;

        var timeSinceLastBeat = beatLength - timeUntilNextBeat;

        if (ReferenceEquals(uninheritedPoint, lastTimingPoint) && beatIndex == lastBeat)
            return;

        if (Math.Abs(timeSinceLastBeat) < 16)
            using (BeginDelayedSequence(-timeSinceLastBeat))
                OnBeat(beatIndex, uninheritedPoint, closestPoint, BeatSource.CurrentAmplitudes);

        lastBeat = beatIndex;
        lastTimingPoint = uninheritedPoint;
    }
}
