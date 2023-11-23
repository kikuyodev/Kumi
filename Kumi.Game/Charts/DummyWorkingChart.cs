using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Charts;

public class DummyWorkingChart : WorkingChart
{
    private readonly TextureStore textures;

    public DummyWorkingChart(AudioManager audio, TextureStore textures)
        : base(new ChartInfo
        {
            Metadata = new ChartMetadata
            {
                Artist = "Please load a chart!",
                Title = "No charts available!"
            }
        }, audio)
    {
        this.textures = textures;

        // We are guaranteed to have a virtual track.
        // To ease usability, ensure the track is available from point of construction.
        LoadChartTrack();
    }

    protected override IChart GetChart() => new Chart();

    protected override Track GetChartTrack() => GetVirtualTrack();

    public override Texture GetBackground() => textures.Get("Backgrounds/default");

    public override Stream? GetStream(string storagePath) => null;
}
