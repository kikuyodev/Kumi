using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class ChartStatistics : CompositeDrawable
{
    private const float width = 250;
    
    [Resolved]
    private Bindable<APIChart?> selectedChart { get; set; } = null!;
    
    public ChartStatistics()
    {
        Width = width;
        AutoSizeAxes = Axes.Y;
    }

    private SimpleStatisticRow noteCount = null!;
    private SimpleStatisticRow songLength = null!;
    
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Bottom = -10 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 5,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colours.Gray(0.07f).Opacity(0.75f)
                    }
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(8),
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4),
                Children = new Drawable[]
                {
                    noteCount = new SimpleStatisticRow
                    {
                        Label = "Notes"
                    },
                    songLength = new SimpleStatisticRow
                    {
                        Label = "Length"
                    },
                    new BpmStatisticRow()
                }
            }
        };
        
        selectedChart.BindValueChanged(_ => updateDisplay(), true);
    }

    private void updateDisplay()
    {
        noteCount.Value = selectedChart.Value?.Statistics.NoteCount.ToString("N0") ?? "-";
        songLength.Value = TimeSpan.FromMilliseconds(selectedChart.Value?.Statistics.DrainLength ?? 0).ToString(@"mm\:ss");
    }
}
