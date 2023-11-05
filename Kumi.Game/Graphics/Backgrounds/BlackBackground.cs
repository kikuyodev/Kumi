using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osuTK.Graphics;

namespace Kumi.Game.Graphics.Backgrounds;

public partial class BlackBackground : BackgroundScreen
{
    [BackgroundDependencyLoader]
    private void load(IRenderer renderer)
    {
        Background background;
        SetBackgroundImmediately(background = new Background());

        background.Colour = Color4.Black;
        background.SetBackground(renderer.WhitePixel);
    }
}
