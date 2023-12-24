using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Input;
using Kumi.Game.Screens.Select.Mods;
using osu.Framework.Allocation;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Screens.Select;

public partial class ModSelectButton : KumiButton, IKeyBindingHandler<GlobalAction>
{
    [Resolved]
    private ModSelectionOverlay modSelectionOverlay { get; set; } = null!;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Action = () => modSelectionOverlay.ToggleVisibility();
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case GlobalAction.ToggleModSelect:
                modSelectionOverlay.ToggleVisibility();
                break;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }
}
