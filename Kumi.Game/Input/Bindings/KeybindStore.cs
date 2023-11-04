using Kumi.Game.Database;
using osu.Framework.Allocation;

namespace Kumi.Game.Input;

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
        return;
    }
}
