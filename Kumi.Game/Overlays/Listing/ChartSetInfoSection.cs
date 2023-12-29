using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays.Listing;

public partial class ChartSetInfoSection : VisibilityContainer
{
    private const float height = 200;
    
    public ChartSetInfoSection()
    {
        RelativeSizeAxes = Axes.X;
        Masking = true;

        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            Height = 200
        };
    }

    protected override void PopIn()
    {
        this.ResizeHeightTo(height, 200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.ResizeHeightTo(0, 200, Easing.OutQuint);
    }
}
