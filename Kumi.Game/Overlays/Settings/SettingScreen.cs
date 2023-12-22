using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays.Settings;

public partial class SettingScreen : Container
{
    private FillFlowContainer content = null!;

    protected override Container<Drawable> Content => content;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        
        InternalChild = content = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new(0, 12),
        };
    }
}
