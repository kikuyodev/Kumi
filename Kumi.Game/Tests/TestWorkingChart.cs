using Kumi.Game.Charts;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Tests;

public class TestWorkingChart : WorkingChart
{
    private readonly IChart chart;
    private readonly TextureStore textures;
    
    public TestWorkingChart(IChart chart, AudioManager audio, TextureStore textures)
        : base(chart.ChartInfo, audio)
    {
        this.chart = chart;
        this.textures = textures;

        LoadChartTrack();
    }

    protected override IChart? GetChart() => chart;

    protected override Track? GetChartTrack() => GetVirtualTrack();

    public override Texture? GetBackground() => textures.Get("Backgrounds/default");

    public override Stream? GetStream(string storagePath) => null;
}
