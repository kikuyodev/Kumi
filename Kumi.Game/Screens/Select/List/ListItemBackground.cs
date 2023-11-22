using Kumi.Game.Charts;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Select.List;

public partial class ListItemBackground : BufferedContainer
{
    private readonly IWorkingChart chart;

    public ListItemBackground(IWorkingChart chart)
        : base(cachedFrameBuffer: true)
    {
        this.chart = chart;
    }

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        RedrawOnScale = false;
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Texture = chart.GetBackground() ?? store.Get("Backgrounds/bg1.jpg"),
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientHorizontal(Color4.Black, Color4.Black.Opacity(0.5f)),
                Alpha = 0.75f
            }
        };
    }
}
