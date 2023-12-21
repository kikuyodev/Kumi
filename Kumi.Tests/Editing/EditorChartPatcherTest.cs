using System.Text;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Formats;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit;
using NUnit.Framework;

namespace Kumi.Tests.Editing;

[TestFixture]
public class EditorChartPatcherTest
{
    private EditorChartPatcher patcher;
    private EditorChart current;

    [SetUp]
    public void Setup()
    {
        patcher = new EditorChartPatcher(current = new EditorChart(new Chart()));
    }

    [Test]
    public void TestPatchNoChanges()
    {
        runTest(new Chart());
    }

    [Test]
    public void TestAddNote()
    {
        var patch = new Chart
        {
            Notes =
            {
                new DrumHit(1000)
            }
        };

        runTest(patch);
    }

    [Test]
    public void TestInsertNote()
    {
        current.AddRange(new Note[]
        {
            new DrumHit(1000),
            new DrumHit(3000),
        });

        var patch = new Chart
        {
            Notes =
            {
                current.Notes[0],
                new DrumHit(2000),
                current.Notes[1],
            }
        };

        runTest(patch);
    }

    [Test]
    public void TestDeleteNote()
    {
        current.AddRange(new Note[]
        {
            new DrumHit(1000),
            new DrumHit(2000),
            new DrumHit(3000),
        });

        var patch = new Chart
        {
            Notes =
            {
                current.Notes[0],
                current.Notes[2],
            }
        };

        runTest(patch);
    }

    [Test]
    public void TestChangeStartTime()
    {
        current.AddRange(new Note[]
        {
            new DrumHit(1000),
            new DrumHit(2000),
            new DrumHit(3000),
        });

        var patch = new Chart
        {
            Notes =
            {
                new DrumHit(500),
                current.Notes[1],
                current.Notes[2],
            }
        };

        runTest(patch);
    }

    [Test]
    public void TestAddMultipleNotes()
    {
        current.AddRange(new Note[]
        {
            new DrumHit(1000),
            new DrumHit(2000),
            new DrumHit(3000),
        });

        var patch = new Chart
        {
            Notes =
            {
                new DrumHit(500),
                current.Notes[0],
                new DrumHit(1500),
                current.Notes[1],
                new DrumHit(2250),
                new DrumHit(2500),
                current.Notes[2],
                new DrumHit(3500),
            }
        };
        
        runTest(patch);
    }

    [Test]
    public void TestDeleteMultipleNotes()
    {
        current.AddRange(new Note[]
        {
            new DrumHit(500),
            new DrumHit(1000),
            new DrumHit(1500),
            new DrumHit(2000),
            new DrumHit(2500),
            new DrumHit(3000),
            new DrumHit(3500),
        });
        
        var patch = new Chart
        {
            Notes =
            {
                current.Notes[0],
                current.Notes[3],
                current.Notes[6],
            }
        };
        
        runTest(patch);
    }

    private void runTest(Chart patch)
    {
        patch = decode(encode(patch));

        patcher.Patch(encode((Chart) current.PlayableChart), encode(patch));

        var currentStr = Encoding.ASCII.GetString(encode((Chart) current.PlayableChart));
        var patchStr = Encoding.ASCII.GetString(encode(patch));

        Assert.That(currentStr, Is.EqualTo(patchStr));
    }

    private byte[] encode(Chart chart)
    {
        using var stream = new MemoryStream();
        new ChartEncoder().Encode(chart, stream);
        var data = stream.ToArray();
        stream.Close();

        return data;
    }

    private Chart decode(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return new ChartDecoder().Decode(stream);
    }
}
