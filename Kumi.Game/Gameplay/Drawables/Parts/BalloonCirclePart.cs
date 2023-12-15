using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay.Drawables.Parts;

public partial class BalloonCirclePart : ConstrictedScalingContainer
{
    private readonly bool withBorder;
    
    public BalloonCirclePart(bool withBorder = true)
    {
        this.withBorder = withBorder;
        PreferredSize = new Vector2(72);
    }

    protected override void UpdateAfterChildren()
    {
        Width = DrawSize.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new CircularContainer
        {
            Masking = true,
            MaskingSmoothness = 1f,
            BorderColour = Color4.White.Opacity(withBorder ? 1f : 0.01f),
            BorderThickness = 6.5f,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(1f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = DrawableBalloon.BALLOON_COLOUR_GRADIENT
            }
        };
    }
}
