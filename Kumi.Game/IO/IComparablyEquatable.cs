namespace Kumi.Game.IO;

/// <summary>
/// An interface for objects that can be compared and equated.
///
/// This interface is used to allow for the use of <see cref="IComparable{T}"/> and <see cref="IEquatable{T}"/> in the same object,
/// so that both interfaces can be used in a single object of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IComparablyEquatable<T> : IComparable<T>, IEquatable<T>
{
}
