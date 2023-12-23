using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingPointFlowList : AlwaysLastFillFlowContainer
{
    [Resolved(name: "selected_point")]
    private Bindable<TimingPoint> selectedPoint { get; set; } = null!;

    public Action<TimingPointType>? CreatePoint;

    public IEnumerable<TimingPoint> TimingPoints
    {
        set
        {
            Clear();

            if (!value.Any())
                return;

            foreach (var point in value)
                Add(new DrawableTimingPoint(point));
        }
    }

    public TimingPointFlowList()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 2);

        AutoSizeDuration = 200;
        AutoSizeEasing = Easing.OutQuint;

        CreateComponent = createButtons;
    }

    private Drawable createButtons()
    {
        return new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding { Horizontal = 12 },
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                ColumnDimensions = new []
                {
                    new Dimension(),
                    new Dimension(),
                },
                RowDimensions = new[]
                {
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new KumiButton
                        {
                            Text = "Add Uninherited",
                            Margin = new MarginPadding { Right = 8 },
                            Action = () => CreatePoint?.Invoke(TimingPointType.Uninherited)
                        },
                        new KumiButton
                        {
                            Text = "Add Inherited",
                            Margin = new MarginPadding { Left = 8 },
                            Action = () => CreatePoint?.Invoke(TimingPointType.Inherited)
                        }
                    }
                }
            }
        };
    }
}
