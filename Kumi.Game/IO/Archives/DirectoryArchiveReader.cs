namespace Kumi.Game.IO.Archives;

public class DirectoryArchiveReader : ArchiveReader
{
    public DirectoryArchiveReader(string path)
        : base(path)
    {
    }

    public override Stream? GetStream(string name) => File.OpenRead(GetFullPath(name));

    public string GetFullPath(string filename) => Path.Combine(ArchivePath!, filename);

    public override void Dispose()
    {
    }

    public override IEnumerable<string> FileNames
        => Directory.GetFiles(ArchivePath!, "*", SearchOption.AllDirectories).Select(f => f.Replace(ArchivePath!, string.Empty).Trim(Path.DirectorySeparatorChar)).ToArray();
}
