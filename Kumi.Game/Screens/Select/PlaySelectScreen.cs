using Kumi.Game.Charts;
using Kumi.Game.Screens.Edit;
using Kumi.Game.Screens.Play;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Select;

public partial class PlaySelectScreen : SelectScreen
{
    public override MenuItem[] CreateContextMenuItemsForChartSet(ChartSetInfo chartSetInfo)
        => base.CreateContextMenuItemsForChartSet(chartSetInfo)
           .Concat(new[]
            {
                new MenuItem("Play", () => FinaliseSelectionInternal(chartSetInfo.Charts.First())),
                new MenuItem("Edit", () =>
                {
                    Chart.Value = Manager.GetWorkingChart(chartSetInfo.Charts.First());
                    this.Push(new Editor());
                }),
            })
           .ToArray();

    public override MenuItem[] CreateContextMenuItemsForChart(ChartInfo chartInfo)
        => base.CreateContextMenuItemsForChart(chartInfo)
           .Concat(new[]
            {
                new MenuItem("Play", () => FinaliseSelectionInternal(chartInfo)),
                new MenuItem("Edit", () =>
                {
                    Chart.Value = Manager.GetWorkingChart(chartInfo);
                    this.Push(new Editor());
                }),
            })
           .ToArray();

    protected override bool FinaliseSelectionInternal(ChartInfo chartInfo)
    {
        this.Push(new PlayerLoader());
        return true;
    }
}
