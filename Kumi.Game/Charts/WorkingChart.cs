using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;

namespace Kumi.Game.Charts;

public abstract class WorkingChart : IWorkingChart
{
    public readonly ChartInfo ChartInfo;
    public readonly ChartSetInfo ChartSetInfo;
    
    public ChartMetadata? Metadata => ChartInfo.Metadata;
    
    private AudioManager audioManager { get; }
    
    private CancellationTokenSource? loadCancellationSource = new CancellationTokenSource();
    
    private readonly object chartFetchLock = new object();

    private Track? track;
    private Waveform? waveform;

    protected WorkingChart(ChartInfo chartInfo, AudioManager audioManager)
    {
        this.audioManager = audioManager;

        ChartInfo = chartInfo;
        ChartSetInfo = chartInfo.ChartSet ?? new ChartSetInfo();
    }

    #region Resource getters
    
    protected virtual Waveform? GetWaveform() => new Waveform(null);
    protected abstract IChart? GetChart();
    protected abstract Track? GetChartTrack();

    public abstract Texture? GetBackground();
    
    #endregion

    #region Async load control

    public void BeginAsyncLoad() => loadChartAsync();

    public void CancelAsyncLoad()
    {
        lock (chartFetchLock)
        {
            loadCancellationSource?.Cancel();
            loadCancellationSource = new CancellationTokenSource();

            if (chartLoadTask?.IsCompleted != true)
                chartLoadTask = null;
        }
    }

    #endregion
    
    #region Track

    public bool TrackLoaded => track != null;

    public Track LoadChartTrack()
    {
        track = GetChartTrack() ?? GetVirtualTrack(1000);
        
        waveform?.Dispose();
        waveform = null;

        return track;
    }
    
    public void PrepareTrackForPreview(bool looping, double offsetFromPreviewPoint = 0)
    {
        Track.Looping = looping;
        Track.RestartPoint = Metadata!.PreviewTime;

        if (Track.RestartPoint == -1)
        {
            if (!Track.IsLoaded)
            {
                // force length to be populated (https://github.com/ppy/osu-framework/issues/4202)
                Track.Seek(Track.CurrentTime);
            }

            Track.RestartPoint = 0.4f * Track.Length;
        }

        Track.RestartPoint += offsetFromPreviewPoint;
    }
    
    /// <summary>
    /// Attempts to transfer the audio track to a target working chart, if valid for transferring.
    /// Used as an optimisation to avoid reload / track swap across charts.
    /// </summary>
    /// <param name="target">The target working chart to transfer this track to.</param>
    /// <returns>Whether the track has been transferred to the <paramref name="target"/>.</returns>
    public virtual bool TryTransferTrack(WorkingChart target)
    {
        if (ChartInfo?.AudioEquals(target.ChartInfo) != true || Track.IsDummyDevice)
            return false;

        target.track = Track;
        return true;
    }
    
    public Track Track
    {
        get
        {
            if (!TrackLoaded)
                throw new InvalidOperationException($"Cannot access {nameof(Track)} without first calling {nameof(LoadChartTrack)}.");

            return track!;
        }
    }
    
    protected Track GetVirtualTrack(double emptyLength = 0)
    {
        const double excess_length = 1000;
        var length = excess_length + emptyLength;

        return audioManager.Tracks.GetVirtual(length);
    }

    #endregion
    
    #region Waveform

    public Waveform? Waveform => waveform ?? GetWaveform()!;

    #endregion
    
    #region Chart

    public virtual bool ChartLoaded => chartLoadTask?.IsCompleted ?? false;

    public IChart Chart
    {
        get
        {
            try
            {
                return loadChartAsync().GetResultSafely();
            }
            catch (AggregateException ae)
            {
                // This is the exception that is generally expected here, which occurs via natural cancellation of the asynchronous load
                if (ae.InnerExceptions.FirstOrDefault() is TaskCanceledException)
                    return null!;
                
                Logger.Error(ae, "Chart failed to load");
                return null!;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Chart failed to load");
                return null!;
            }
        }
    }

    private Task<IChart>? chartLoadTask;

    private Task<IChart> loadChartAsync()
    {
        lock (chartFetchLock)
        {
            return chartLoadTask ??= Task.Factory.StartNew(() =>
            {
                var c = GetChart() ?? new Chart();
                c.ChartInfo = ChartInfo;
                return c;
            });
        }
    }
    
    #endregion

    public abstract Stream? GetStream(string storagePath);
    
    ChartInfo IWorkingChart.ChartInfo => ChartInfo;
}
