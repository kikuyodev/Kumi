using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Charts.Drawables;

public partial class ChartBackgroundSprite : Sprite
{
    private readonly IWorkingChart chart;

    public ChartBackgroundSprite(IWorkingChart chart)
    {
        this.chart = chart;
    }

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        var background = chart.GetBackground();
        Texture = background ?? store.Get("Backgrounds/bg1.jpg");
    }
}
