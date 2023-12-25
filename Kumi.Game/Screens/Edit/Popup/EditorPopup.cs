using Kumi.Game.Input;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Kumi.Game.Screens.Edit.Popup;

public partial class EditorPopup : VisibilityContainer, IKeyBindingHandler<GlobalAction>
{
    protected virtual bool CanBeExited => true;

    public virtual bool BlockPositionalInput => true;
    public virtual bool BlockScreenWideMouse => BlockPositionalInput;

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => BlockScreenWideMouse || base.ReceivePositionalInputAt(screenSpacePos);

    public EditorPopup()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

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

    protected override void UpdateState(ValueChangedEvent<Visibility> state)
    {
        switch (state.NewValue)
        {
            case Visibility.Visible:
            case Visibility.Hidden when CanBeExited:
                base.UpdateState(state);
                break;
            
            case Visibility.Hidden when !CanBeExited:
                State.Value = Visibility.Visible;
                break;
        }
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
