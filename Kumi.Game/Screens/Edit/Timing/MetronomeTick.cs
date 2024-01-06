using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Utils;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class MetronomeTick : BeatContainer
{
    private Sample? high;
    private Sample? low;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        high = audio.Samples.Get("metronome/metronome-hi");
        low = audio.Samples.Get("metronome/metronome-lo");
    }

    protected override void OnBeat(int beatIndex, UninheritedTimingPoint uninheritedTimingPoint, TimingPoint timingPoint, ChannelAmplitudes amplitudes)
    {
        if (!IsBeatSynced)
            return;

        var channel = beatIndex % uninheritedTimingPoint.TimeSignature.Numerator == 0 ? high?.GetChannel() : low?.GetChannel();
        
        if (channel == null)
            return;

        channel.Frequency.Value = RNG.NextDouble(0.98f, 1.02f);
        channel.Play();
    }
}
