using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Components;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public partial class ScrollSpeedSection : ConfigurationSection
{
    protected override LocalisableString Title => "Relative Scroll Speed";

    public ScrollSpeedSection(TimingPoint point)
        : base(point)
    {
    }

    private EditorSliderBar<float> sliderBar = null!;

    protected override Drawable[] CreateContent()
        => new Drawable[]
        {
            sliderBar = new EditorSliderBar<float>
            {
                RelativeSizeAxes = Axes.None,
                Width = 200,
                BarHeight = 16,
                Current = new BindableFloat
                {
                    MinValue = 0.5f,
                    MaxValue = 2.5f,
                    Value = Point.RelativeScrollSpeed
                },
                BackgroundColour = Colours.Gray(0.1f),
                BarColour = Colours.BLUE_ACCENT
            }
        };

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        sliderBar.Current.BindValueChanged(e =>
        {
            Point.RelativeScrollSpeed = e.NewValue;
        });
    }
}
