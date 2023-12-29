using Kumi.Game.Charts;
using Kumi.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.IO.Stores;

namespace Kumi.Game.Audio;

public partial class TrackPreviewManager : Component
{
    private readonly IAdjustableAudioComponent mainTrackComponent;
    
    private readonly BindableDouble muteBindable = new BindableDouble();

    private ITrackStore trackStore = null!;

    protected TrackManagerTrackPreview? CurrentTrack;
    
    public TrackPreviewManager(IAdjustableAudioComponent mainTrackComponent)
    {
        this.mainTrackComponent = mainTrackComponent;
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        trackStore = audio.GetTrackStore(new OnlineStore());
    }

    public TrackPreview Get(IChartSetInfo chartSetInfo)
    {
        var track = createTrackPreview(chartSetInfo, trackStore);

        track.Started += () => Schedule(() =>
        {
            CurrentTrack?.Stop();
            CurrentTrack = track;
            mainTrackComponent.AddAdjustment(AdjustableProperty.Volume, muteBindable);
        });

        track.Stopped += () => Schedule(() =>
        {
            if (CurrentTrack != track)
                return;

            CurrentTrack = null;
            mainTrackComponent.RemoveAdjustment(AdjustableProperty.Volume, muteBindable);
        });

        return track;
    }
    
    public void StopPlaying()
    {
        CurrentTrack?.Stop();
    }

    private TrackManagerTrackPreview createTrackPreview(IChartSetInfo chartSetInfo, ITrackStore trackStore)
        => new TrackManagerTrackPreview(chartSetInfo, trackStore);

    public partial class TrackManagerTrackPreview : TrackPreview
    {
        private readonly IChartSetInfo chartSetInfo;
        private readonly ITrackStore trackStore;

        [Resolved]
        private IAPIConnectionProvider api { get; set; } = null!;

        public TrackManagerTrackPreview(IChartSetInfo chartSetInfo, ITrackStore trackStore)
        {
            this.chartSetInfo = chartSetInfo;
            this.trackStore = trackStore;
        }

        protected override Track? GetTrack()
            => trackStore.Get($"{api.EndpointConfiguration.WebsiteUri}/cdn/previews/{chartSetInfo.OnlineID}");
    }
}
