using Kumi.Game.Input.Bindings;
using osu.Framework.Input.Bindings;

namespace Kumi.Game.Input;

public partial class GlobalKeybindContainer : RealmBackedKeyBindingContainer<GlobalAction>
{
    public override IEnumerable<IKeyBinding> DefaultKeyBindings => Array.Empty<KeyBinding>();

    public GlobalKeybindContainer()
        : base(KeybindType.Global)
    {
    }
}
