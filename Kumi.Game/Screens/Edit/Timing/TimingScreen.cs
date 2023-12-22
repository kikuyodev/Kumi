using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Timeline;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingScreen : EditorScreenWithTimeline
{
    [Cached(name: "current_point")]
    public readonly Bindable<TimingPoint> CurrentPoint = new Bindable<TimingPoint>();

    [Resolved]
    private EditorClock? editorClock { get; set; }

    public TimingScreen()
        : base(EditorScreenMode.Timing)
    {
    }

    protected override Drawable CreateMainContent()
    {
        return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding
            {
                Top = 12,
                Bottom = 12 + BottomBarTimeline.HEIGHT + 12,
            },
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Horizontal = 12 },
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 5,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.05f)
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Horizontal = 12 },
                        Child = new TimingPointList()
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (editorClock != null)
        {
            var nearestPoint = EditorChart.TimingPoints
               .LastOrDefault(t => t.StartTime <= editorClock.CurrentTime);

            if (nearestPoint != null)
                CurrentPoint.Value = (TimingPoint) nearestPoint;
        }
    }
}
