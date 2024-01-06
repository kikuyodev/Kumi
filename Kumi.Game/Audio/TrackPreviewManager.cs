using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Charts;
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
    
    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;
    
    public TrackPreviewManager(IAdjustableAudioComponent mainTrackComponent)
    {
        this.mainTrackComponent = mainTrackComponent;
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        trackStore = audio.GetTrackStore(new OnlineStore());
    }

    public TrackPreview Get(IAPIChartMetadata chartMetadata)
    {
        var track = createTrackPreview(chartMetadata, trackStore);

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

    private TrackManagerTrackPreview createTrackPreview(IAPIChartMetadata chartMetadata, ITrackStore trackStore)
        => new TrackManagerTrackPreview(chartMetadata, trackStore, api);

    public partial class TrackManagerTrackPreview : TrackPreview
    {
        private readonly IAPIChartMetadata chartMetadata;
        private readonly ITrackStore trackStore;
        private readonly IAPIConnectionProvider api;

        public TrackManagerTrackPreview(IAPIChartMetadata chartMetadata, ITrackStore trackStore, IAPIConnectionProvider api)
        {
            this.chartMetadata = chartMetadata;
            this.trackStore = trackStore;
            this.api = api;
        }

        protected override Track? GetTrack()
            => trackStore.Get($"{api.EndpointConfiguration.WebsiteUri}/cdn/previews/{chartMetadata.Id}");
    }
}
