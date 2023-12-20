using Kumi.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class SummarySliderBar<T> : SliderBar<T>
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public ColourInfo ForegroundColour
    {
        get => foreground.Colour;
        set => foreground.Colour = value;
    }
    
    private readonly Circle foreground;

    public SummarySliderBar()
    {
        Children = new Drawable[]
        {
            new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.08f)
            },
            foreground = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Width = 0
            }
        };
    }
    
    protected override void UpdateValue(float value)
    {
        foreground.ResizeWidthTo(value, 200, Easing.OutQuint);
    }
}
