using Kumi.Game.Charts;
using Kumi.Game.Screens.Edit;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Select;

public partial class EditSelectScreen : SelectScreen
{
    public override MenuItem[] CreateContextMenuItemsForChartSet(ChartSetInfo chartSetInfo)
        => base.CreateContextMenuItemsForChartSet(chartSetInfo)
           .Concat(new[]
            {
                new MenuItem("Edit", () => FinaliseSelectionInternal(chartSetInfo.Charts.First())),
            })
           .ToArray();

    public override MenuItem[] CreateContextMenuItemsForChart(ChartInfo chartInfo)
        => base.CreateContextMenuItemsForChart(chartInfo)
           .Concat(new[]
            {
                new MenuItem("Edit", () => FinaliseSelectionInternal(chartInfo)),
            })
           .ToArray();

    protected override Drawable CreateWedge()
        => new EditWedge
        {
            OnImport = v => FinaliseSelection(v.ChartInfo)
        };

    protected override bool FinaliseSelectionInternal(ChartInfo chartInfo)
    {
        this.Push(new Editor());
        return true;
    }
}
