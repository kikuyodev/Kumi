using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class SliderBarComponent<T> : CompositeDrawable, IHasCurrentValue<T>
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public Bindable<T> Current { get; set; } = new Bindable<T>();

    private readonly KumiSliderBar<T> sliderBar;
    
    public SliderBarComponent()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChild = sliderBar = new KumiSliderBar<T>
        {
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Width = SettingOverlay.TEXT_BOX_WIDTH,
            RelativeSizeAxes = Axes.None,
            BackgroundColour = Colours.Gray(0.1f),
            BarColour = Colours.BLUE_ACCENT_LIGHT
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        sliderBar.Current.BindTo(Current);
    }
}
