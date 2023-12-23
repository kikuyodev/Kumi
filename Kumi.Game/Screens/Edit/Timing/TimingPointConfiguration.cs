using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Timing.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingPointConfiguration : VisibilityContainer
{
    protected override bool StartHidden => true;

    private readonly TimingPoint point;

    public TimingPointConfiguration(TimingPoint point)
    {
        this.point = point;

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        AutoSizeDuration = 200;
        AutoSizeEasing = Easing.OutQuint;

        Padding = new MarginPadding { Horizontal = 12 };
    }

    private Container content = null!;
    private FillFlowContainer leftConfiguration = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.07f)
                }
            },
            content = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding { Top = TimingPointSummary.HEIGHT + 12, Bottom = 12 },
                Masking = true,
                CornerRadius = 5,
                Child = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Horizontal = 12 },
                    Children = new Drawable[]
                    {
                        leftConfiguration = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(24, 0),
                            Children = new Drawable[]
                            {
                                new StartTimeSection(point),
                            }
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(24, 0),
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Children = new Drawable[]
                            {
                                new VolumeSection(point)
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight
                                },
                                new ScrollSpeedSection(point)
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight
                                }
                            }
                        },
                    }
                }
            }
        };

        if (point.PointType == TimingPointType.Uninherited)
            leftConfiguration.Add(new UninheritedSection(point));
    }

    protected override void PopIn()
    {
        this.FadeIn(200, Easing.OutQuint);
        content.FadeIn(200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        content.FadeOut();
        this.FadeOut(200, Easing.OutQuint);
    }
}
