using Kumi.Game.Charts.Formats;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Timings;
using NUnit.Framework;
using osu.Framework.Utils;

namespace Kumi.Tests.Charts.Formats;

[TestFixture]
public class ChartDecoderTest
{
    private ChartDecoder? decoder;

    [SetUp]
    public void SetUp()
    {
        decoder?.Dispose();
        decoder = new ChartDecoder();
    }

    [Test]
    public void TestDecode()
    {
        // TODO write more verbose tests
        using var testResource = TestResources.OpenResource("Charts/test_decode.kch");
        var chart = decoder!.Decode(testResource);

        Assert.IsTrue(chart.IsProcessed);

        Assert.AreEqual(2, chart.Notes.Count);
        Assert.AreEqual(1, chart.Events.Count);
        Assert.AreEqual(1, chart.TimingPoints.Count);

        Assert.AreEqual("Sweet Sweet Cendrillion Drug", chart.Metadata.Title);
        Assert.AreEqual("Sweet Sweet Cendrillion Drug", chart.Metadata.TitleRomanised);
        Assert.AreEqual("Oni", chart.ChartInfo.DifficultyName);
        Assert.AreEqual("audio.mp3", chart.Metadata.AudioFile);
        Assert.AreEqual("bg.jpg", chart.Metadata.BackgroundFile);

        Assert.IsTrue(Precision.AlmostEquals(1.0f, chart.ChartInfo.InitialScrollSpeed));
        Assert.IsTrue(Precision.AlmostEquals(195f, ((UninheritedTimingPoint) chart.TimingPoints.First()).BPM));

        Assert.AreEqual(1246, chart.Notes[0].StartTime);
        Assert.AreEqual(NoteType.Don, chart.Notes[0].Type);
        Assert.AreEqual(1554, chart.Notes[1].StartTime);
        Assert.AreEqual(NoteType.Kat, chart.Notes[1].Type);
    }
}
