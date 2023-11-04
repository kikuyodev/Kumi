namespace Kumi.Game.Database;

/// <summary>
/// An interface for a class that stores default values for a type.
/// </summary>
public interface IStoreDefaults<T>
    where T : class
{
    /// <summary>
    /// A function that returns the default values for this instance.
    /// </summary>
    public IEnumerable<T> GetDefaultValues();
}
