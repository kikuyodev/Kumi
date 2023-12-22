using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class CheckboxComponent : ClickableContainer, IHasCurrentValue<bool>
{
    public Bindable<bool> Current { get; set; } = new Bindable<bool>();

    private readonly CircularContainer checkbox;

    public CheckboxComponent()
    {
        RelativeSizeAxes = Axes.Both;

        Child = checkbox = new CircularContainer
        {
            Size = new Vector2(20),
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Masking = true,
            BorderThickness = 3f,
            BorderColour = Colours.BLUE_ACCENT_LIGHT,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true
            }
        };

        Action = () => Current.Value = !Current.Value;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Current.BindValueChanged(onValueChanged, true);
    }

    private void onValueChanged(ValueChangedEvent<bool> val)
    {
        if (val.NewValue)
            checkbox.TransformTo(nameof(BorderThickness), 10f, 200, Easing.OutQuint);
        else
            checkbox.TransformTo(nameof(BorderThickness), 3f, 200, Easing.OutQuint);
    }
}
