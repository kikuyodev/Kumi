using Kumi.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Control;

public partial class BasicNotification : Notification
{
    private IconUsage icon = FontAwesome.Solid.InfoCircle;

    public IconUsage Icon
    {
        get => icon;
        set
        {
            icon = value;
            if (spriteIcon != null)
                spriteIcon.Icon = icon;
        }
    }

    public ColourInfo BackgroundColour
    {
        get => background?.Colour ?? Colours.Gray(0.5f);
        set
        {
            if (background != null)
                background.Colour = value;
        }
    }

    private Box? background;
    private SpriteIcon? spriteIcon;

    protected override Drawable CreateIcon()
    {
        return new Container
        {
            Size = new Vector2(40),
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.5f)
                },
                spriteIcon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = icon,
                    Size = new Vector2(20),
                    Colour = Color4.White
                }
            }
        };
    }
}
