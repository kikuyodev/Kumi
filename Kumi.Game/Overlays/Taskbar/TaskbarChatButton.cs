using Kumi.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Overlays.Taskbar;

public partial class TaskbarChatButton : TaskbarIconButton, IKeyBindingHandler<GlobalAction>
{
    [Resolved]
    private ChatOverlay? Overlay { get; set; }
    
    public TaskbarChatButton()
    {
        Icon = FontAwesome.Solid.Comment;
        Action = toggleOverlay;
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        switch (e.Action)
        {
            case GlobalAction.ToggleChat:
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
