namespace Kumi.Game.IO.Archives;

public class SingleFileArchiveReader : ArchiveReader
{
    public SingleFileArchiveReader(string path)
        : base(path)
    {
    }

    public override Stream? GetStream(string name) => File.OpenRead(ArchivePath!);

    public override void Dispose()
    {
    }

    public override IEnumerable<string> FileNames => new[] { Path.GetFileName(ArchivePath!) };
}

