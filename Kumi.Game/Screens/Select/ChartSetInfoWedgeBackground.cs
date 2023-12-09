using Kumi.Game.Charts;
using Kumi.Game.Charts.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Select;

public partial class ChartSetInfoWedgeBackground : CompositeDrawable
{
    private readonly IWorkingChart chart;

    public ChartSetInfoWedgeBackground(IWorkingChart chart)
    {
        this.chart = chart;
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new BufferedContainer(cachedFrameBuffer: true)
        {
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientVertical(Color4.White, Color4.White.Opacity(0.2f)),
                    Children = new Drawable[]
                    {
                        new ChartBackgroundSprite(chart)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill
                        }
                    }
                }
            }
        };
    }
}
