using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Graphics.Sprites;

public partial class GlowingSpriteText : Container, IHasText
{
    private readonly SpriteText text;
    private readonly SpriteText blurredText;

    public LocalisableString Text
    {
        get => text.Text;
        set => text.Text = blurredText.Text = value;
    }

    public FontUsage Font
    {
        get => text.Font;
        set => text.Font = value;
    }

    public FontUsage BlurredFont
    {
        get => text.Font;
        set => blurredText.Font = value;
    }
    
    public Vector2 TextSize
    {
        get => text.Size;
        set => text.Size = blurredText.Size = value;
    }
    
    public ColourInfo TextColour
    {
        get => text.Colour;
        set => text.Colour = value;
    }
    
    public ColourInfo GlowColour
    {
        get => blurredText.Colour;
        set => blurredText.Colour = value;
    }
    
    public Vector2 Spacing
    {
        get => text.Spacing;
        set => text.Spacing = blurredText.Spacing = value;
    }
    
    public bool UseFullGlyphHeight
    {
        get => text.UseFullGlyphHeight;
        set => text.UseFullGlyphHeight = blurredText.UseFullGlyphHeight = value;
    }
    
    public Bindable<string> Current
    {
        get => text.Current;
        set => text.Current = blurredText.Current = value;
    }

    public GlowingSpriteText()
    {
        AutoSizeAxes = Axes.Both;

        Children = new Drawable[]
        {
            new BufferedContainer(cachedFrameBuffer: true)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                BlurSigma = new Vector2(5),
                RedrawOnScale = false,
                RelativeSizeAxes = Axes.Both,
                Blending = BlendingParameters.Additive,
                Size = new Vector2(3f),
                Children = new Drawable[]
                {
                    blurredText = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            },
            text = new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
