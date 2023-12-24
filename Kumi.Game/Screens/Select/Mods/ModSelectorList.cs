using Kumi.Game.Gameplay.Mods;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Select.Mods;

public partial class ModSelectorList : FillFlowContainer
{
    private static readonly Mod[] all_mods =
    {
        new ModDoubleTime(),
        new ModHalfTime()
    };

    private readonly Dictionary<Mod, DrawableModSelector> modSelectors = new Dictionary<Mod, DrawableModSelector>();

    [Resolved]
    private BindableList<Mod> selectedMods { get; set; } = null!;

    public ModSelectorList()
    {
        Direction = FillDirection.Vertical;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Spacing = new Vector2(0, 4);

        Padding = new MarginPadding { Horizontal = 12 };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        all_mods.ForEach(addMod);
        ensureSelectionIsCorrect();

        selectedMods.BindCollectionChanged((_, _) => ensureSelectionIsCorrect());
    }

    private void addMod(Mod mod)
    {
        var selector = new DrawableModSelector(mod);
        selector.RequestSelection += performSelection;

        modSelectors.Add(mod, selector);
        Add(selector);
    }

    private void ensureSelectionIsCorrect()
    {
        foreach (var mod in modSelectors)
        {
            var isSelected = selectedMods.Contains(mod.Key);
            mod.Value.State = isSelected;
        }
    }

    private void performSelection(Mod mod)
    {
        var selector = modSelectors[mod];
        selector.State = !modSelectors[mod].State;

        if (selector.State)
            selectedMods.Add(mod);
        else
            selectedMods.Remove(mod);

        foreach (var incompatibleMod in mod.IncompatibleMods)
        {
            var incompatibleModInstance = all_mods.FirstOrDefault(m => m.GetType() == incompatibleMod);
            if (incompatibleModInstance == null)
                continue;
            
            modSelectors[incompatibleModInstance].State = false;
            selectedMods.Remove(incompatibleModInstance);
        }
    }
}
