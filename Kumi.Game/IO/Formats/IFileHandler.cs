namespace Kumi.Game.IO.Formats;

/// <summary>
/// A interface for a file format handler, which can be used to parse a file into a <see cref="T"/>; or to write a <see cref="T"/> to a file.
/// </summary>
/// <typeparam name="T">The type being handled.</typeparam>
/// <typeparam name="TSection">The secionds of the file as an enum.</typeparam>
public interface IFileHandler<T, TSection> : IFileHandler<T>
    where T : new()
    where TSection : struct
{
    /// <summary>
    /// The current section being parsed.
    /// </summary>
    public TSection CurrentSection { get; set; }
}

public interface IFileHandler<T>
    where T : new()
{
    /// <summary>
    /// The current version of the file format being handled.
    /// </summary>
    public int Version { get; }
    
    /// <summary>
    /// The current object being handled.
    /// </summary>
    public T Current { get; set; }
}