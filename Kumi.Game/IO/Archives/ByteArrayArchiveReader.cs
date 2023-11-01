namespace Kumi.Game.IO.Archives;

/// <summary>
/// Allows reading a single file from the provided byte array.
/// </summary>
public class ByteArrayArchiveReader : ArchiveReader
{
    private readonly byte[] content;

    public ByteArrayArchiveReader(byte[] content, string path)
        : base(path)
    {
        this.content = content;
    }

    public override Stream? GetStream(string name) => new MemoryStream(content);

    public override void Dispose()
    {
    }

    public override IEnumerable<string> FileNames => new[] { Path.GetFileName(ArchivePath!) };
}

