using Kumi.Game.Database;

namespace Kumi.Game.Input.Bindings;

/// <summary>
/// A store that stores all of the keybinds for the game.
/// </summary>
public class KeybindStore : RealmBackedDefaultStore<Keybind>
{
    public KeybindStore(RealmAccess realmAccess) : base(realmAccess)
    {
    }

    public override void AssignDefaults()
    {
    }
}
