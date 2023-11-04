namespace Kumi.Game.IO;

public interface IFileInfo
{
    /// <summary>
    /// The computed SHA-256 hash of the file.
    /// </summary>
    string Hash { get; }
}
