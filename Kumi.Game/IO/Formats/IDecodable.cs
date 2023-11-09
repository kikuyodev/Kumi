namespace Kumi.Game.IO.Formats;

public interface IDecodable
{
    /// <summary>
    /// The version of the file.
    /// </summary>
    int Version { get; set; }

    /// <summary>
    /// Whether the file has been processed.
    /// </summary>
    bool IsProcessed { get; set; }
}
