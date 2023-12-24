using Kumi.Game.Gameplay.Mods;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays.Select;

public partial class ModSelectionOverlay : Container
{
    [Cached]
    public Bindable<IReadOnlyList<Mod>> SelectedMods { get; } = new Bindable<IReadOnlyList<Mod>>(Array.Empty<Mod>());
    
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
    }
}
