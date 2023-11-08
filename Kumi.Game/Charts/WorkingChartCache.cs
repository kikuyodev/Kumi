using Kumi.Game.Database;
using Kumi.Game.Extensions;
using Kumi.Game.IO;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Dummy;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Lists;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Statistics;

namespace Kumi.Game.Charts;

public class WorkingChartCache : IChartResourceProvider, IWorkingChartCache
{
    private readonly WeakList<ChartManagerWorkingChart> workingCache = new WeakList<ChartManagerWorkingChart>();

    // Chart files may specify this filename to denote that they don't have an audio track.
    private const string virtual_track_filename = "virtual";

    /// <summary>
    /// A default representation of a WorkingChart to use when no chart is available.
    /// </summary>
    public readonly WorkingChart? DefaultChart;

    private readonly AudioManager audioManager;
    private readonly LargeTextureStore largeTextureStore;
    private readonly ITrackStore trackStore;
    private readonly IResourceStore<byte[]> files;
    private readonly IResourceStore<byte[]> resources;

    private readonly GameHost? host;

    public WorkingChartCache(ITrackStore trackStore, AudioManager audioManager, IResourceStore<byte[]> resources, IResourceStore<byte[]> files,
                             WorkingChart? defaultChart = null,
                             GameHost? host = null)
    {
        DefaultChart = defaultChart;

        this.audioManager = audioManager;
        this.host = host;
        this.files = files;
        this.resources = resources;
        largeTextureStore = new LargeTextureStore(host?.Renderer ?? new DummyRenderer(), host?.CreateTextureLoaderStore(files));
        this.trackStore = trackStore;
    }

    public void Invalidate(ChartSetInfo info)
    {
        foreach (var c in info.Charts)
            Invalidate(c);
    }

    public void Invalidate(ChartInfo info)
    {
        lock (workingCache)
        {
            var working = workingCache.FirstOrDefault(w => info.Equals(w.ChartInfo));
            
            if (working != null)
            {
                Logger.Log($"Invalidating working chart cache for {info}");
                workingCache.Remove(working);
                OnInvalidated?.Invoke(working);
            }
        }
    }

    public event Action<WorkingChart>? OnInvalidated;

    public virtual WorkingChart? GetWorkingChart(ChartInfo? chartInfo)
    {
        if (chartInfo?.ChartSet == null)
            return DefaultChart;

        lock (workingCache)
        {
            var working = workingCache.FirstOrDefault(w => chartInfo.Equals(w.ChartInfo));

            if (working != null)
                return working;

            chartInfo = chartInfo.Detach();
            workingCache.Add(working = new ChartManagerWorkingChart(chartInfo, this));

            GlobalStatistics.Get<int>("Charts", $"Cached {nameof(WorkingChart)}s").Value = workingCache.Count();

            return working;
        }
    }

    #region IResourceStorageProvider

    TextureStore IChartResourceProvider.LargeTextureStore => largeTextureStore;
    ITrackStore IChartResourceProvider.Tracks => trackStore;
    IRenderer IStorageResourceProvider.Renderer => host?.Renderer ?? new DummyRenderer();
    AudioManager IStorageResourceProvider.AudioManager => audioManager;
    RealmAccess IStorageResourceProvider.RealmAccess => null!;
    IResourceStore<byte[]> IStorageResourceProvider.Files => files;
    IResourceStore<byte[]> IStorageResourceProvider.Resources => resources;
    IResourceStore<TextureUpload>? IStorageResourceProvider.CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore) => host?.CreateTextureLoaderStore(underlyingStore);

    #endregion

    private class ChartManagerWorkingChart : WorkingChart
    {
        private readonly IChartResourceProvider resources;

        public ChartManagerWorkingChart(ChartInfo chartInfo, IChartResourceProvider resources)
            : base(chartInfo, resources.AudioManager!)
        {
            this.resources = resources;
        }

        protected override IChart? GetChart()
        {
            if (ChartInfo.Path == null)
                return new Chart { ChartInfo = ChartInfo };

            try
            {
                var fileStorePath = ChartSetInfo.GetPathForFile(ChartInfo.Path);
                var stream = GetStream(fileStorePath!);

                if (stream == null)
                {
                    Logger.Log($"Chart failed to load (file {ChartInfo.Path} not found on disk at expected location {fileStorePath}).", level: LogLevel.Error);
                    return null;
                }

                // return null for now until we have a chart decoder.
                // TODO: chart decoder.
                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Chart failed to load");
                return null;
            }
        }
        
        public override Texture? GetBackground()
        {
            if (string.IsNullOrEmpty(Metadata.BackgroundFile))
                return null;

            try
            {
                var fileStorePath = ChartSetInfo.GetPathForFile(Metadata.BackgroundFile);
                var texture = resources.LargeTextureStore.Get(fileStorePath);
                
                if (texture == null)
                {
                    Logger.Log($"Chart background failed to load (file {Metadata.BackgroundFile} not found on disk at expected location {fileStorePath}).", level: LogLevel.Error);
                    return null;
                }

                return texture;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Background failed to load");
                return null;
            }
        }

        protected override Track? GetChartTrack()
        {
            if (string.IsNullOrEmpty(Metadata.AudioFile))
                return null;

            if (Metadata.AudioFile == virtual_track_filename)
                return null!;

            try
            {
                var fileStorePath = ChartSetInfo.GetPathForFile(Metadata.AudioFile);
                var track = resources.Tracks.Get(fileStorePath);
                
                if (track == null)
                {
                    Logger.Log($"Chart track failed to load (file {Metadata.AudioFile} not found on disk at expected location {fileStorePath}).", level: LogLevel.Error);
                    return null;
                }

                return track;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Track failed to load");
                return null;
            }
        }

        protected override Waveform? GetWaveform()
        {
            if (string.IsNullOrEmpty(Metadata.AudioFile))
                return null;

            if (Metadata.AudioFile == virtual_track_filename)
                return null!;

            try
            {
                var fileStorePath = ChartSetInfo.GetPathForFile(Metadata.AudioFile);
                var trackData = GetStream(fileStorePath!);

                if (trackData == null)
                {
                    Logger.Log($"Chart waveform failed to load (file {Metadata.AudioFile} not found on disk at expected location {fileStorePath}).", level: LogLevel.Error);
                    return null;
                }

                return new Waveform(trackData);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Waveform failed to load");
                return null;
            }
        }

        public override Stream? GetStream(string storagePath) => resources.Files.GetStream(storagePath);
    }
}
