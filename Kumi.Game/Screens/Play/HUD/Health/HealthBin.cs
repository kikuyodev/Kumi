using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Play.HUD.Health;

public partial class HealthBin : Container
{
    private readonly CircularContainer bar;

    public readonly BindableDouble Current = new BindableDouble();
    public double MinValue = 0;
    public double MaxValue = 1;

    private Color4 barColour = Color4Extensions.FromHex("E5F6FF");

    public Color4 BarColour
    {
        get => barColour;
        set
        {
            barColour = value;
            bar.Colour = value;
        }
    }

    private Color4 glowColour = Color4Extensions.FromHex("00AAFF").Opacity(0.25f);

    public Color4 GlowColour
    {
        get => glowColour;
        set
        {
            glowColour = value;
            bar.EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = value.Opacity(0.25f),
                Radius = 6,
                Roundness = 3
            };
        }
    }

    public HealthBin()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 4;
        
        Children = new Drawable[]
        {
            new CircularContainer
            {
                BorderColour = Color4.White.Opacity(0.1f),
                BorderThickness = 1,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true
                }
            },
            bar = new CircularContainer
            {
                RelativeSizeAxes = Axes.Both,
                Height = 0,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Glow,
                    Colour = glowColour.Opacity(0.25f),
                    Radius = 6,
                    Roundness = 3,
                },
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true
                }
            }
        };

        Current.BindValueChanged(v => UpdateValue((float) v.NewValue), true);
    }

    protected void UpdateValue(float value)
    {
        var trueValue = Interpolation.ValueAt(value, 0d, 1d, MinValue, MaxValue);
        var clamped = (float) Math.Clamp(trueValue, 0, 1);
        bar.Height = clamped;

        if (clamped > 0.5f)
            bar.Colour = BarColour;
        else
            bar.Colour = BarColour.Opacity(clamped * 2);
    }
}
