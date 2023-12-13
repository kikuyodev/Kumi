using Kumi.Game.Input.Bindings;
using osu.Framework.Input.Bindings;

namespace Kumi.Game.Input;

public partial class GlobalKeybindContainer : RealmBackedKeyBindingContainer<GlobalAction>
{
    public GlobalKeybindContainer()
        : base(KeybindType.Global)
    {
    }

    public override IEnumerable<IKeyBinding> DefaultKeyBindings
        => new[]
        {
            new KeyBinding(new KeyCombination(InputKey.Control, InputKey.T), GlobalAction.ToggleTaskbar),
            new KeyBinding(new KeyCombination(InputKey.Control, InputKey.N), GlobalAction.ToggleNotifications),
            
            new KeyBinding(InputKey.Escape, GlobalAction.Back),
            new KeyBinding(InputKey.ExtraMouseButton1, GlobalAction.Back),
            
            new KeyBinding(InputKey.Enter, GlobalAction.Select)
        };
}
