using osu.Framework.Bindables;

namespace Kumi.Game.Bindables;

public class LazyBindable<T>
{
    private Bindable<T>? backingBindable;

    private T backingValue;

    public Bindable<T> Bindable => backingBindable ??= new Bindable<T>(defaultValue) { Value = backingValue };

    public T Value
    {
        get => backingBindable != null ? backingBindable.Value : backingValue;
        set
        {
            if (backingBindable != null)
                backingBindable.Value = value;
            else
                backingValue = value;
        }
    }

    private readonly T defaultValue;

    public LazyBindable(T value = default!)
    {
        backingValue = defaultValue = value;
        backingBindable = null;
    }
}
