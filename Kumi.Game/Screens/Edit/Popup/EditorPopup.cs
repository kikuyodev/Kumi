using Kumi.Game.Input;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Screens.Edit.Popup;

public partial class EditorPopup : VisibilityContainer, IKeyBindingHandler<GlobalAction>
{
    protected override void PopIn()
    {
        this.ScaleTo(1, 200, Easing.OutQuint);
        this.FadeIn(200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.ScaleTo(0.75f, 200, Easing.OutQuint);
        this.FadeOut(200, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        if (State.Value == Visibility.Hidden)
            return false;

        if (e.Action == GlobalAction.Back)
        {
            Hide();
            return true;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }
}
