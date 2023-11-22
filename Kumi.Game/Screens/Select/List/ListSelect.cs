using Kumi.Game.Charts;
using Kumi.Game.Database;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace Kumi.Game.Screens.Select.List;

public partial class ListSelect : BasicScrollContainer
{
    private readonly FillFlowContainer<ListItemGroup> content;

    private ListItemGroup currentlySelected = null!;

    public ListSelect()
    {
        RelativeSizeAxes = Axes.Both;

        Child = content = new FillFlowContainer<ListItemGroup>
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 12),
        };
    }

    [BackgroundDependencyLoader]
    private void load(ChartManager manager)
    {
        var charts = manager.GetAllUsableCharts().Detach();

        foreach (var chart in charts)
        {
            var item = new ListItemGroup(chart);
            item.RequestSelect = () =>
            {
                currentlySelected.Selected.Value = false;
                currentlySelected = item;
                currentlySelected.Selected.Value = true;

                return true;
            };

            content.Add(item);
        }
        
        // select random chart
        currentlySelected = content.Children[RNG.Next(0, content.Children.Count)];
        currentlySelected.Selected.Value = true;
    }
}
