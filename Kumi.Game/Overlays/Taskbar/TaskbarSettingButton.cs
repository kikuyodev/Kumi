using Kumi.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Overlays.Taskbar;

public partial class TaskbarSettingButton : TaskbarIconButton, IKeyBindingHandler<GlobalAction>
{
    [Resolved]
    private SettingOverlay? settingOverlay { get; set; }
    
    public TaskbarSettingButton()
    {
        Icon = FontAwesome.Solid.Cog;
        Action = toggleOverlay;
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        switch (e.Action)
        {
            case GlobalAction.ToggleNotifications:
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
        if (settingOverlay?.State.Value == Visibility.Hidden)
            settingOverlay.Show();
        else
            settingOverlay?.Hide();
    }
}
