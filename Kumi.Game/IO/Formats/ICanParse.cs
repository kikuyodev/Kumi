namespace Kumi.Game.IO.Formats;

/// <summary>
/// An interface for objects that can be parsed from a value of type <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">The type that can be parsed/</typeparam>
public interface ICanParse<T>
{
    /// <summary>
    /// Parses this object from <paramref name="input" />.
    /// </summary>
    /// <param name="input">The input.</param>
    void ParseFrom(T input);
}
