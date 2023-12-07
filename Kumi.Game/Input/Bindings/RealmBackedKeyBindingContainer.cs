using Kumi.Game.Database;
using osu.Framework.Allocation;
using osu.Framework.Input.Bindings;

namespace Kumi.Game.Input.Bindings;

/// <summary>
/// A key binding container that is backed by a Realm database.
/// </summary>
/// <typeparam name="T">The actions this container handles.</typeparam>
public abstract partial class RealmBackedKeyBindingContainer<T> : KeyBindingContainer<T>, IHasDefaults<Keybind>
    where T : struct
{
    /// <summary>
    /// The type of keybinds this container handles.
    /// </summary>
    public KeybindType Type { get; }

    protected RealmBackedKeyBindingContainer(
        KeybindType type,
        SimultaneousBindingMode simultaneousMode = SimultaneousBindingMode.None,
        KeyCombinationMatchingMode matchingMode = KeyCombinationMatchingMode.Any)
        : base(simultaneousMode, matchingMode)
    {
        Type = type;
    }

    [Resolved]
    private RealmAccess realm { get; set; } = null!;

    protected override void LoadComplete()
    {
        realm.Subscribe<Keybind>(b => b.Type == Type, (sender, changes) =>
        {
            if (changes == null)
                return;

            ReloadMappings(sender.AsEnumerable());
        });
        
        ReloadMappings();
    }

    protected override void ReloadMappings() => ReloadMappings(realm.Run(r => r.All<Keybind>().Where(b => b.Type == Type)));

    private void ReloadMappings(IEnumerable<Keybind> keybinds) =>
        KeyBindings = keybinds.Where(b => b.Type == Type).Select(b => new KeyBinding(b.KeyCombination, b.Action));

    public IEnumerable<Keybind> GetDefaultValues() => DefaultKeyBindings.Select(b => new Keybind(Type, (int) b.Action, b.KeyCombination));
}
