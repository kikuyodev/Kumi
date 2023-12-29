using Kumi.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Overlays.Taskbar;

public partial class TaskbarListingButton : TaskbarIconButton, IKeyBindingHandler<GlobalAction>
{
    [Resolved]
    private ListingOverlay? Overlay { get; set; }
    
    public TaskbarListingButton()
    {
        Icon = FontAwesome.Solid.Square;
        Action = toggleOverlay;
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        switch (e.Action)
        {
            case GlobalAction.ToggleChartSetListing:
                toggleOverlay();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    private void toggleOverlay()
    {
        if (Overlay?.State.Value == Visibility.Hidden)
            Overlay.Show();
        else
            Overlay?.Hide();
    }
}
