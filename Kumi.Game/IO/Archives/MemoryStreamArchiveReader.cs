namespace Kumi.Game.IO.Archives;

/// <summary>
/// Allows reading a single file from the provided memory stream.
/// </summary>
public class MemoryStreamArchiveReader : ArchiveReader
{
    private readonly MemoryStream stream;

    public MemoryStreamArchiveReader(MemoryStream stream, string path)
        : base(path)
    {
        this.stream = stream;
    }

    public override Stream? GetStream(string name) => new MemoryStream(stream.ToArray(), 0, (int) stream.Length);

    public override void Dispose()
    {
    }

    public override IEnumerable<string> FileNames => new[] { Path.GetFileName(ArchivePath!) };
}
