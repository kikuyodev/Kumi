using Kumi.Game.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Control;

public partial class MusicProgressBar : SliderBar<double>
{
    public Action<double>? OnSeek;

    private readonly Circle fill;
    private readonly Circle background;

    public Color4 FillColour
    {
        set => fill.Colour = value;
    }
    
    public Color4 BackgroundColour
    {
        set => background.Colour = value;
    }

    public double EndTime
    {
        set => CurrentNumber.MaxValue = value;
    }

    public double CurrentTime
    {
        set => CurrentNumber.Value = value;
    }
    
    private readonly bool isSeekable;

    public override bool HandlePositionalInput => isSeekable;
    public override bool HandleNonPositionalInput => isSeekable;

    public MusicProgressBar(bool isSeekable)
    {
        this.isSeekable = isSeekable;

        CurrentNumber.MinValue = 0;
        CurrentNumber.MaxValue = 1;
        RelativeSizeAxes = Axes.X;

        Children = new Drawable[]
        {
            background = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.1f).Opacity(0.75f),
            },
            fill = new Circle
            {
                RelativeSizeAxes = Axes.Y,
                Colour = Colours.BLUE_LIGHTER
            }
        };
    }

    protected override void UpdateValue(float value)
    {
        fill.Width = value * UsableWidth;
    }

    protected override void OnUserChange(double value)
        => OnSeek?.Invoke(value);
}
