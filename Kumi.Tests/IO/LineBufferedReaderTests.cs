using System.Text;
using Kumi.Game.IO;
using NUnit.Framework;

namespace Kumi.Tests.IO;

[TestFixture]
public class LineBufferedReaderTests
{
    [Test]
    public void ReadLineByLine()
    {
        const string contents = """
                                hi
                                hewo
                                meow
                                """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
        using var reader = new LineBufferedReader(stream);

        Assert.AreEqual("hi", reader.ReadLine());
        Assert.AreEqual("hewo", reader.ReadLine());
        Assert.AreEqual("meow", reader.ReadLine());
        Assert.IsNull(reader.ReadLine());
    }

    [Test]
    public void PeekLineMultipleTimes()
    {
        const string contents = """
                                hi
                                hewo
                                meow
                                """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
        using var reader = new LineBufferedReader(stream);

        Assert.AreEqual("hi", reader.PeekLine());
        Assert.AreEqual("hi", reader.ReadLine());
        Assert.AreEqual("hewo", reader.PeekLine());
        Assert.AreEqual("hewo", reader.PeekLine());
        Assert.AreEqual("hewo", reader.ReadLine());
        Assert.AreEqual("meow", reader.PeekLine());
        Assert.AreEqual("meow", reader.ReadLine());
        Assert.IsNull(reader.PeekLine());
    }

    [Test]
    public void PeekAtEnd()
    {
        const string contents = """
                                hi
                                hewo
                                """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
        using var reader = new LineBufferedReader(stream);

        Assert.AreEqual("hi", reader.ReadLine());
        Assert.AreEqual("hewo", reader.ReadLine());
        Assert.IsNull(reader.PeekLine());
        Assert.IsNull(reader.ReadLine());
    }

    [Test]
    public void PeekReadLineOnEmptyStream()
    {
        using var stream = new MemoryStream();
        using var reader = new LineBufferedReader(stream);

        Assert.IsNull(reader.PeekLine());
        Assert.IsNull(reader.ReadLine());
    }

    [Test]
    public void ReadToEnd()
    {
        const string contents = """
                                hi
                                hewo
                                meow
                                """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
        using var reader = new LineBufferedReader(stream);

        Assert.AreEqual(contents, reader.ReadToEnd());
    }

    [Test]
    public void ReadToEndWithReadsAndPeeks()
    {
        const string content = """
                               line 1
                               line 2
                               line 3
                               line 4
                               """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new LineBufferedReader(stream);

        Assert.AreEqual("line 1", reader.ReadLine());
        Assert.AreEqual("line 2", reader.PeekLine());

        var endingLines = reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(3, endingLines.Length);
        Assert.AreEqual("line 2", endingLines[0]);
        Assert.AreEqual("line 3", endingLines[1]);
        Assert.AreEqual("line 4", endingLines[2]);
    }
}
