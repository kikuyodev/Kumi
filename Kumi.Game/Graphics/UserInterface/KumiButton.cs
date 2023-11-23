using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK.Graphics;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiButton : Button
{
    private readonly Container container;
    private readonly Box background;
    private readonly SpriteText spriteText;

    private LocalisableString text;

    public LocalisableString Text
    {
        get => text;
        set
        {
            text = value;
            spriteText.Text = text;
        }
    }

    private Color4 backgroundColour = Color4Extensions.FromHex("F94826");
    
    public Color4 BackgroundColour
    {
        get => backgroundColour;
        set
        {
            backgroundColour = value;
            background.Colour = backgroundColour;
        }
    }
    
    private FontUsage font = KumiFonts.GetFont(weight: FontWeight.Medium, size: 14);
    
    public FontUsage Font
    {
        get => font;
        set
        {
            font = value;
            spriteText.Font = font;
        }
    }

    private bool important;

    public bool Important
    {
        get => important;
        set
        {
            if (important == value)
                return;

            if (value)
            {
                BorderColour = new ColourInfo
                {
                    TopLeft = Color4.White.Opacity(0.25f),
                    TopRight = Color4.White.Opacity(0.25f),
                    BottomLeft = Color4.White.Opacity(0.1f * 0.25f),
                    BottomRight = Color4.White.Opacity(0.1f * 0.25f)
                };
            }
            else
            {
                BorderColour = Color4.Transparent;
            }
            
            important = value;
        }
    }

    public new float Height
    {
        get => container.Height;
        set
        {
            container.AutoSizeAxes = Axes.None;
            container.Padding = new MarginPadding(0);
            container.Height = value;
        }
    }

    public KumiButton()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Masking = true;
        CornerRadius = 5;
        BorderThickness = 1.5f;
        BorderColour = Color4.Transparent;
        
        Children = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = backgroundColour
            },
            container = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding
                {
                    Vertical = 4
                },
                Children = new Drawable[]
                {
                    spriteText = new SpriteText
                    {
                        Text = Text,
                        Font = font,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
        
        Enabled.BindValueChanged(e =>
        {
            this.FadeColour(e.NewValue ? Color4.White : Color4.White.Darken(0.5f), 100, Easing.OutQuint);
        }, true);
    }

    protected override bool OnHover(HoverEvent e)
    {
        background.FadeColour(backgroundColour.Lighten(0.25f), 200, Easing.OutQuint);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeColour(backgroundColour, 200, Easing.OutQuint);
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!Enabled.Value)
            return false;
        
        background.FlashColour(backgroundColour.Lighten(0.9f), 500, Easing.OutQuint);
        return base.OnClick(e);
    }
}
