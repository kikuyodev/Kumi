using Kumi.Game.Charts;
using Kumi.Game.Charts.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
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
    private void load()
    {
        RedrawOnScale = false;
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            new ChartBackgroundSprite(chart)
            {
                RelativeSizeAxes = Axes.Both,
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
