using Kumi.Game.Charts;
using Kumi.Game.Charts.Events;
using Kumi.Game.Charts.Formats;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Timings;
using Kumi.Game.Models;
using NUnit.Framework;

namespace Kumi.Tests.Charts.Formats;

[TestFixture]
public class ChartEncoderTest
{
    private ChartEncoder? encoder;

    [SetUp]
    public void SetUp()
    {
        encoder?.Dispose();
        encoder = new ChartEncoder();
    }

    [Test]
    public void TestEncode()
    {
        var chart = createChart();

        var filename = TestResources.GetTemporaryFilename("kch");
        using var writer = TestResources.OpenWritableTemporaryFile(filename);
        encoder!.Encode(chart, writer);

        var contents = new StreamReader(TestResources.OpenReadableTemporaryFile(filename)).ReadToEnd();
        var expected = new StreamReader(TestResources.OpenResource("Charts/test_encode.kch")).ReadToEnd();

        Assert.AreEqual(expected, contents);

        using var decoder = new ChartDecoder();
        var decoded = decoder.Decode(TestResources.OpenReadableTemporaryFile(filename));

        Assert.IsTrue(decoded.IsProcessed);

        Assert.AreEqual(chart.Events.Count, decoded.Events.Count);
        Assert.AreEqual(chart.TimingPoints.Count, decoded.TimingPoints.Count);
        Assert.AreEqual(chart.Notes.Count, decoded.Notes.Count);
    }

    private Chart createChart()
    {
        var chartSet = createChartSet();
        var chart = new Chart();
        chart.ChartInfo = chartSet.Charts[0];

        // create events
        chart.Events.Add(new SetMediaEvent("bg.jpg"));

        // create timings
        chart.TimingPoints.Add(new UninheritedTimingPoint(0)
        {
            BPM = 120.5f,
            TimeSignature = new TimeSignature(4, 4)
        });

        // create notes
        chart.Notes.Add(new DrumHit(0)
        {
            Type = { Value = NoteType.Don }
        });
        chart.Notes.Add(new DrumHit(1000)
        {
            Type = { Value = NoteType.Kat }
        });

        return chart;
    }

    private ChartSetInfo createChartSet()
    {
        var metadata = new ChartMetadata
        {
            Artist = "Test Artist",
            Title = "Test Title",
            Creator = new RealmAccount { Username = "Test Author" }
        };

        var chartInfo = new ChartInfo
        {
            DifficultyName = "Test Difficulty",
            InitialScrollSpeed = 1.2f,
            Metadata = metadata.DeepClone()
        };

        var chartSetInfo = new ChartSetInfo(new[] { chartInfo });
        chartInfo.ChartSet = chartSetInfo;

        return chartSetInfo;
    }
}
