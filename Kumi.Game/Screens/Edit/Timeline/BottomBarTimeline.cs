using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class BottomBarTimeline : GridContainer
{
    private const float padding = 8;
    
    [Resolved]
    private EditorClock clock { get; set; } = null!;

    private SpriteText timeText = null!;

    public BottomBarTimeline()
    {
        RelativeSizeAxes = Axes.X;
        Height = 24;

        ColumnDimensions = new[]
        {
            new Dimension(GridSizeMode.Absolute, 74),
            new Dimension(GridSizeMode.Absolute, padding),
            new Dimension(),
            new Dimension(GridSizeMode.Absolute, padding),
            new Dimension(GridSizeMode.Absolute, 150),
            new Dimension(GridSizeMode.Absolute, padding),
            new Dimension(GridSizeMode.AutoSize)
        };
        
        RowDimensions = new[]
        {
            new Dimension()
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Content = new[]
        {
            new[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 5,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.05f),
                        },
                        timeText = new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new Vector2(-1, 0),
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 12).With(fixedWidth: true),
                            Colour = Colours.GRAY_6,
                            Padding = new MarginPadding { Horizontal = 8 }
                        }
                    }
                },
                Empty(), // padding
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 5,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.05f),
                        },
                        new TimelineSummary
                        {
                            Padding = new MarginPadding { Horizontal = 8 },
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                Empty(), // padding
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = 5,
                            Masking = true,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colours.Gray(0.05f),
                            },
                        },
                        new PlaybackControl
                        {
                            Padding = new MarginPadding { Horizontal = 8 },
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
                Empty(), // padding
                new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    CornerRadius = 5,
                    Masking = true,
                    Action = () =>
                    {
                        // todo: test
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.BLUE_ACCENT_LIGHT,
                        },
                        new SpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, size: 12),
                            Colour = Colours.Gray(0.05f),
                            Padding = new MarginPadding { Horizontal = 16 },
                            Text = "Test"
                        }
                    }
                },
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        var timespan = TimeSpan.FromMilliseconds(clock.CurrentTime);
        timeText.Text = $"{(timespan < TimeSpan.Zero ? "-" : string.Empty)}{(int) timespan.TotalMinutes:00}:{timespan:ss\\:fff}";
    }
}
