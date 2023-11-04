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
    
    [Resolved]
    private GlobalKeybindContainer keybindContainer { get; set; }
    
    public override IEnumerable<Keybind> GetDefaultValues()
    {
        var keybinds = new List<Keybind>();
        
        foreach (var binding in keybindContainer.DefaultKeyBindings)
        {
            keybinds.Add(new Keybind(KeybindType.Global, (int)binding.Action, binding.KeyCombination));
        }
        
        return keybinds;
    }
}
