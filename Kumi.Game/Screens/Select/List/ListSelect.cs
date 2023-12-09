using Kumi.Game.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace Kumi.Game.Screens.Select.List;

public partial class ListSelect : Container
{
    private readonly FillFlowContainer<ListItemGroup> content;
    private ListItemGroup currentlySelected = null!;

    public readonly Bindable<ChartInfo> SelectedChart = new Bindable<ChartInfo>();

    public ListSelect()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

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
        var charts = manager.GetAllUsableCharts();
        charts = charts.Where(c => c.Charts.Any()).ToList();

        foreach (var chart in charts)
        {
            var item = new ListItemGroup(chart);
            item.RequestSelect = _ =>
            {
                currentlySelected.Selected.Value = false;
                currentlySelected = item;
                currentlySelected.Selected.Value = true;
                return true;
            };
            
            item.OnSelectionChanged = c => SelectedChart.Value = c;

            content.Add(item);
        }
        
        // select random chart
        currentlySelected = content.Children[RNG.Next(0, content.Children.Count)];
        currentlySelected.Selected.Value = true;
    }
}
