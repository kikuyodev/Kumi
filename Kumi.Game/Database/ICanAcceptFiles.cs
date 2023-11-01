namespace Kumi.Game.Database;

/// <summary>
/// A class that can accept files to be imported.
/// </summary>
public interface ICanAcceptFiles
{
    /// <summary>
    /// Import one or more items from the filesystem.
    /// </summary>
    /// <param name="paths">The files which should be imported.</param>
    Task Import(params string[] paths);

    /// <summary>
    /// Import one or more items from an archive.
    /// </summary>
    /// <param name="tasks">The tasks which will be imported.</param>
    Task Import(ImportTask[] tasks);
    
    /// <summary>
    /// An array of file extensions that this importer can handle.
    /// </summary>
    IEnumerable<string> HandledFileExtensions { get; }
}
