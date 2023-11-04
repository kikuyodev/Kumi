using osu.Framework.Input.Bindings;

namespace Kumi.Game.Input;

public class GlobalKeybindContainer : RealmBackedKeyBindingContainer<GlobalAction>
{

    public override IEnumerable<IKeyBinding> DefaultKeyBindings => new KeyBinding[]
    {

    };
}
