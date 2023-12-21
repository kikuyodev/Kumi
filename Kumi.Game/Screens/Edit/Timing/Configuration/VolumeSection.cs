using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Components;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public partial class VolumeSection : ConfigurationSection
{
    protected override LocalisableString Title => "Volume";

    public VolumeSection(TimingPoint point)
        : base(point)
    {
    }

    private EditorSliderBar<int> sliderBar = null!;

    protected override Drawable[] CreateContent()
        => new Drawable[]
        {
            sliderBar = new EditorSliderBar<int>
            {
                RelativeSizeAxes = Axes.None,
                Width = 200,
                BarHeight = 16,
                Current = new BindableInt
                {
                    MinValue = 0,
                    MaxValue = 100,
                    Value = Point.Volume
                },
                BackgroundColour = Colours.Gray(0.1f),
                BarColour = Colours.RED_ACCENT
            }
        };

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        sliderBar.Current.BindValueChanged(e =>
        {
            Point.Volume = e.NewValue;
        });
    }
}
