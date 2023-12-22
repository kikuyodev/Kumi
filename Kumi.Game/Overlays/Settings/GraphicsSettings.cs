using Kumi.Game.Overlays.Settings.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Settings;

public partial class GraphicsSettings : SettingScreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new LayoutSection(),
            new RendererSettings(),
        };
    }
}
