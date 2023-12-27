using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;

namespace Kumi.Game.Audio;

public partial class DrawablePausableSample : AudioContainer<DrawableSample>
{
    public bool RequestedPlaying { get; private set; }

    private readonly ISample? sample;

    private DrawableSample drawableSample = null!;
    private SampleChannel? sampleChannel;

    public DrawablePausableSample(ISample? sample)
    {
        this.sample = sample;
    }

    private readonly IBindable<bool> samplePaused = new Bindable<bool>();

    [BackgroundDependencyLoader]
    private void load(ISamplePausablePlayback? playback)
    {
        if (sample != null)
            Add(drawableSample = new DrawableSample(sample, false));

        if (playback != null)
        {
            samplePaused.BindTo(playback.SamplePaused);
            samplePaused.BindValueChanged(onSamplePausedChanged, true);
        }
    }

    private ScheduledDelegate? scheduledStart;

    private void onSamplePausedChanged(ValueChangedEvent<bool> paused)
    {
        if (!RequestedPlaying)
            return;

        if (paused.NewValue)
            Stop();
        else
        {
            scheduledStart = Schedule(() =>
            {
                if (RequestedPlaying)
                    play();
            });
        }
    }

    public void Play()
    {
        cancelPendingStart();
        RequestedPlaying = true;

        if (samplePaused.Value)
            return;

        play();
    }

    public void Stop()
    {
        cancelPendingStart();
        RequestedPlaying = false;

        stop();
    }

    private void play()
    {
        sampleChannel = drawableSample.GetChannel();
        sampleChannel.Play();
    }

    private void stop()
    {
        sampleChannel?.Stop();
        sampleChannel = null;
    }

    private void cancelPendingStart()
    {
        scheduledStart?.Cancel();
        scheduledStart = null;
    }
}
