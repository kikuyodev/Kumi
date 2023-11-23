using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Charts;

public interface IWorkingChart
{
    ChartInfo ChartInfo { get; }

    public bool ChartLoaded { get; }

    public bool TrackLoaded { get; }

    IChart? Chart { get; }

    Texture? GetBackground();

    Waveform? Waveform { get; }

    Track? Track { get; }

    Track? LoadChartTrack();

    Stream? GetStream(string storagePath);

    void BeginAsyncLoad();

    void CancelAsyncLoad();

    void PrepareTrackForPreview(bool looping, double offsetFromPreviewPoint = 0);
}
