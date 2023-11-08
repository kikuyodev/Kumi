using osu.Framework.Extensions;
using osu.Framework.IO.Stores;

namespace Kumi.Game.IO.Archives;

/// <summary>
/// An abstracted reader for an archive, usually a zip file; but can be any other format, including a folder.
/// </summary>
public abstract class ArchiveReader : IResourceStore<byte[]>
{
    /// <summary>
    /// The stream of the archive to read from.
    /// </summary>
    public Stream? Stream { get; }

    /// <summary>
    /// The path to the archive, if any.
    /// </summary>
    public string? ArchivePath { get; }

    /// <summary>
    /// The names of the files available in the archive.
    /// </summary>
    public abstract IEnumerable<string> FileNames { get; }
    
    protected ArchiveReader(Stream stream)
    {
        Stream = stream;
    }

    protected ArchiveReader(string path)
    {
        ArchivePath = Path.GetFullPath(path);
    }
    
    /// <summary>
    /// Gets a stream for the given file name.
    /// </summary>
    /// <param name="name">The file name.</param>
    public abstract Stream? GetStream(string name);
    
    /// <summary>
    /// Gets the available resources in the archive.
    /// </summary>
    public IEnumerable<string> GetAvailableResources() => FileNames;
    
    /// <summary>
    /// Gets the bytes for the given file name.
    /// </summary>
    /// <param name="name">The file name.</param>
    public virtual byte[] Get(string name)
    {
        using var stream = GetStream(name);
        return stream?.ReadAllBytesToArray()!;
    }
    
    /// <summary>
    /// Gets the bytes for the given file name asynchronously.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = new CancellationToken())
    {
        await using var stream = GetStream(name);
        return await stream!.ReadAllBytesToArrayAsync(cancellationToken);
    }
    
    public abstract void Dispose();
}
