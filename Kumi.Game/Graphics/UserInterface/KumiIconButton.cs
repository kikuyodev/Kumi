using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiIconButton : Button
{
    private IconUsage icon;

    public IconUsage Icon
    {
        get => icon;
        set
        {
            icon = value;
            SpriteIcon.Icon = icon;
        }
    }

    private Vector2 iconScale;
    
    public Vector2 IconScale
    {
        get => iconScale;
        set
        {
            iconScale = value;
            SpriteIcon.Scale = iconScale;
        }
    }
    
    private Color4 iconColour = Color4.White;
    
    public Color4 IconColour
    {
        get => iconColour;
        set
        {
            iconColour = value;
            SpriteIcon.Colour = iconColour;
        }
    }

    private Color4 backgroundColour = Colours.Gray(0.1f);
    
    public Color4 BackgroundColour
    {
        get => backgroundColour;
        set
        {
            backgroundColour = value;
            Background.Colour = backgroundColour;
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
        get => Container.Height;
        set
        {
            Container.AutoSizeAxes = Axes.None;
            Container.Padding = new MarginPadding(0);
            Container.Height = value;
            base.Height = value;
        }
    }
    
    protected Container Container = null!;
    protected Box Background = null!;
    protected SpriteIcon SpriteIcon = null!;

    public KumiIconButton()
    {
        Masking = true;
        CornerRadius = 5;
        BorderThickness = 1.5f;
        BorderColour = Color4.Transparent;
        
        Children = CreateContent();
        
        Enabled.BindValueChanged(e =>
        {
            this.FadeColour(e.NewValue ? Color4.White : Color4.White.Darken(0.5f), 100, Easing.OutQuint);
        }, true);
    }

    protected virtual Drawable[] CreateContent()
        => new Drawable[]
        {
            Background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = backgroundColour
            },
            Container = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    SpriteIcon = new SpriteIcon
                    {
                        Icon = icon,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(20)
                    }
                }
            }
        };

    protected override bool OnHover(HoverEvent e)
    {
        Background.FadeColour(backgroundColour.Lighten(0.25f), 200, Easing.OutQuint);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        Background.FadeColour(backgroundColour, 200, Easing.OutQuint);
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!Enabled.Value)
            return false;
        
        Background.FlashColour(backgroundColour.Lighten(0.9f), 500, Easing.OutQuint);
        return base.OnClick(e);
    }
}
