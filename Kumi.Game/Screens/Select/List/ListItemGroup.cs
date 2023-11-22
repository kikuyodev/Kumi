using Kumi.Game.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Select.List;

public partial class ListItemGroup : CompositeDrawable
{
    public readonly ChartSetInfo ChartSetInfo;
    public readonly BindableBool Selected = new BindableBool();
    public readonly Bindable<ChartInfo> SelectedChart = new Bindable<ChartInfo>();

    public Func<bool>? RequestSelect;

    private FillFlowContainer<ChartListItem> chartsContainer = null!;
    private ChartListItem? currentlySelected;

    public ListItemGroup(ChartSetInfo chartSetInfo)
    {
        ChartSetInfo = chartSetInfo;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            AutoSizeDuration = 300,
            AutoSizeEasing = Easing.OutQuint,
            Masking = true,
            Spacing = new Vector2(0, 4),
            Children = new Drawable[]
            {
                new ChartSetListItem(ChartSetInfo)
                {
                    Selected = { BindTarget = Selected },
                    RequestSelect = onSetSelectRequest
                },
                chartsContainer = new FillFlowContainer<ChartListItem>
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(0, 4),
                }
            }
        };

        foreach (var chart in ChartSetInfo.Charts)
            chartsContainer.Add(new ChartListItem(chart)
            {
                RequestSelect = () =>
                {
                    if (!Selected.Value)
                        return;
                    
                    if (currentlySelected != null)
                        currentlySelected.Selected.Value = false;
                    
                    SelectedChart.Value = chart;
                    
                    currentlySelected = chartsContainer.Children.First(c => c.ChartInfo.ID == chart.ID);
                    currentlySelected.Selected.Value = true;
                }
            });
        
        Selected.BindValueChanged(v =>
        {
            chartsContainer.FadeTo(v.NewValue ? 1 : 0);
            
            if (!v.NewValue)
                return;
            
            SelectedChart.Value = ChartSetInfo.Charts.First();

            if (currentlySelected != null)
            {
                currentlySelected.Selected.Value = false;
                currentlySelected = null;
            }
            
            currentlySelected = chartsContainer.Children.First(c => c.ChartInfo.ID == SelectedChart.Value.ID);
            
            if (currentlySelected != null)
                currentlySelected.Selected.Value = true;
        }, true);
    }

    private void onSetSelectRequest()
    {
        if (!RequestSelect?.Invoke() ?? false)
            return;
        
        // TODO: something
    }
}
