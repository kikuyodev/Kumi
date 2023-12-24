using Kumi.Game.Gameplay.Mods;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Screens.Select.Mods;

public partial class ModIndicator : CompositeDrawable
{
    public readonly Mod Mod;

    public ModIndicator(Mod mod)
    {
        Mod = mod;

        Size = new Vector2(32);
        Alpha = 0;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f)
                },
                new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Scale = new Vector2(0.9f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = new BindableDouble(1),
                    InnerRadius = 0.1f,
                    Colour = Colours.BLUE
                },
                new SpriteIcon
                {
                    RelativeSizeAxes = Axes.Both,
                    Scale = new Vector2(0.4f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Colours.GRAY_C,
                    Icon = Mod.Icon,
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(200, Easing.OutQuint);
    }
}
