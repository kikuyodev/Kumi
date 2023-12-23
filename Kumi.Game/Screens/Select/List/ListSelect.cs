using Kumi.Game.Charts;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osuTK;

namespace Kumi.Game.Screens.Select.List;

public partial class ListSelect : FillFlowContainer
{
    private ListItemGroup currentlySelected = null!;
    private ScrollContainer<Drawable> scrollContainer = null!;

    public readonly Bindable<ChartInfo> SelectedChart = new Bindable<ChartInfo>();

    public ListSelect()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 12);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        scrollContainer = (Parent!.Parent as ScrollContainer<Drawable>)!;

        // select random chart
        var groups = Children.OfType<ListItemGroup>().ToArray();
        if (groups.Length == 0)
            return;
        
        currentlySelected = groups[RNG.Next(0, groups.Length)];
        currentlySelected.Selected.Value = true;

        SelectedChart.BindValueChanged(s =>
        {
            if (s.NewValue == null)
                return;

            var group = Children.OfType<ListItemGroup>().FirstOrDefault(g => g.ChartSetInfo.Charts.Contains(s.NewValue));
            if (group == null)
                return;

            currentlySelected.Selected.Value = false;
            currentlySelected = group;
            currentlySelected.Selected.Value = true;
        }, true);
    }

    public void AddChartSet(ChartSetInfo setInfo, Action? onSelected = null)
    {
        var item = new ListItemGroup(setInfo);
        item.RequestSelect = _ =>
        {
            currentlySelected.Selected.Value = false;
            currentlySelected = item;
            currentlySelected.Selected.Value = true;
            onSelected?.Invoke();
            return true;
        };

        item.OnSelectionChanged = c => SelectedChart.Value = c;

        // Inserting one before to let the HalfScrollContainer be the very last child.
        Add(item);
    }
}
