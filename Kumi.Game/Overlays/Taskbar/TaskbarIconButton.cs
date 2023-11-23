using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Overlays.Taskbar;

public partial class TaskbarIconButton : TaskbarButton
{
    private SpriteIcon? spriteIcon;
    
    private IconUsage icon;

    public IconUsage Icon
    {
        get => icon;
        set
        {
            icon = value;

            if (spriteIcon is { IsLoaded: true })
                spriteIcon.Icon = value;
        }
    }

    protected override Drawable CreateContent()
        => spriteIcon = new SpriteIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Icon = icon,
            Size = new(20)
        };
}
