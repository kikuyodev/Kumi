namespace Kumi.Game.IO.Formats;

/// <summary>
/// A interface for a file format handler, which can be used to parse a file into a <see cref="T" />; or to write a
/// <see cref="T" /> to a file.
/// </summary>
/// <typeparam name="T">The type being handled.</typeparam>
/// <typeparam name="TSection">The section of the file as an enum.</typeparam>
public interface IFileHandler<T, TSection> : IFileHandler<T>
    where TSection : struct
{
    /// <summary>
    /// The current section being parsed.
    /// </summary>
    TSection CurrentSection { get; set; }

    /// <summary>
    /// The characters used to denote a section header.
    /// </summary>
    SectionHeaderValues SectionHeader { get; }

    /// <summary>
    /// Whether to close the stream once the file has been processed.
    /// </summary>
    bool CloseStreamUponProcessed { get; }

    /// <summary>
    /// A class containing the start and end characters of a section header.
    /// </summary>
    public class SectionHeaderValues
    {
        public required string Start { get; set; }
        public required string End { get; set; }
    }

    public static SectionHeaderValues DefaultHeaders => new SectionHeaderValues
    {
        Start = "[#",
        End = "]"
    };
}

public interface IFileHandler<T> : IDisposable
{
    /// <summary>
    /// The current version of the file format being handled.
    /// </summary>
    public int Version { get; }

    /// <summary>
    /// The current object being handled.
    /// </summary>
    public T Current { get; set; }

    /// <summary>
    /// Runs further processing on the output, once parsing is complete.
    /// </summary>
    void PostProcess();
}
