using System.Collections.Specialized;
using System.Diagnostics;
using Kumi.Game.Gameplay.Mods;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Select.Mods;

public partial class SelectedModsIndicator : FillFlowContainer
{
    public SelectedModsIndicator()
    {
        Height = 32;
        AutoSizeAxes = Axes.X;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(4, 0);
    }

    [BackgroundDependencyLoader]
    private void load(BindableList<Mod> selectedMods)
    {
        selectedMods.BindCollectionChanged((_, args) => Schedule(() => selectionChanged(args)), true);
    }

    private void selectionChanged(NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                Clear();
                break;
            
            case NotifyCollectionChangedAction.Add:
                Debug.Assert(args.NewItems != null);

                foreach (var mod in args.NewItems)
                {
                    var indicator = new ModIndicator((Mod) mod);
                    Add(indicator);
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                Debug.Assert(args.OldItems != null);
                foreach (var mod in args.OldItems)
                {
                    var indicator = this.FirstOrDefault(i => ((ModIndicator) i).Mod == mod);
                    if (indicator == null)
                        continue;

                    indicator.FadeOut(200).Expire();
                }

                break;
        }
    }
}
