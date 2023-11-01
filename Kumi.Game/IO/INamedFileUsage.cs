namespace Kumi.Game.IO;

public interface INamedFileUsage
{
    /// <summary>
    /// The underlying file.
    /// </summary>
    IFileInfo File { get; }
    
    /// <summary>
    /// The name of the file.
    /// </summary>
    string FileName { get; }
}
