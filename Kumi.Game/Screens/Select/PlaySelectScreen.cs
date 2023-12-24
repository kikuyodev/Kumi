using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit;
using Kumi.Game.Screens.Play;
using Kumi.Game.Screens.Select.Mods;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Select;

public partial class PlaySelectScreen : SelectScreen
{
    protected override Color4 FinaliseButtonColour => Colours.BLUE;
    protected override string FinaliseButtonText => "Play!";

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

    protected override Drawable CreateExtraSelectionButtons()
        => new FillFlowContainer
        {
            Anchor = Anchor.BottomRight,
            Origin = Anchor.BottomRight,
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(8, 0),
            Children = new Drawable[]
            {
                new SelectedModsIndicator(),
                new ModSelectButton
                {
                    RelativeSizeAxes = Axes.None,
                    Height = 32,
                    Width = 125,
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 14),
                    BackgroundColour = Colours.Gray(0.05f),
                    Text = "Select mods",
                }
            }
        };

    protected override bool FinaliseSelectionInternal(ChartInfo chartInfo)
    {
        this.Push(new PlayerLoader());
        return true;
    }
}
