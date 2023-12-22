using Kumi.Game.Input;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiPopover : Popover, IKeyBindingHandler<GlobalAction>
{
    protected override Drawable CreateArrow() => Empty();

    public KumiPopover()
    {
        Background.Colour = Colours.Gray(0.15f);
        Content.Padding = new MarginPadding(10);

        Body.Masking = true;
        Body.CornerRadius = 5;
        Body.Margin = new MarginPadding(8);
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

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
            return false;
        
        return base.OnKeyDown(e);
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        if (State.Value == Visibility.Hidden)
            return false;

        if (e.Action == GlobalAction.Back)
        {
            this.HidePopover();
            return true;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }
}
