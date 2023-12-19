using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Setup.Sections;
using Kumi.Game.Screens.Edit.Timeline;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Screens.Edit.Setup;

public partial class SetupScreen : EditorScreen
{
    public SetupScreen()
        : base(EditorScreenMode.Setup)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Padding = new MarginPadding
        {
            Top = 12 + EditorOverlay.TOP_BAR_HEIGHT + 12,
            Bottom = 12 + BottomBarTimeline.HEIGHT + 12,
            Horizontal = 12
        };

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f)
                },
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension()
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 16),
                                Padding = new MarginPadding(20)
                                {
                                    Right = 16
                                },
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Text = "Metadata",
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                        Colour = Colours.GRAY_C
                                    },
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(0, 32),
                                        Children = new Drawable[]
                                        {
                                            new MetadataSection()
                                        }
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 16),
                                Padding = new MarginPadding(20)
                                {
                                    Right = 16
                                },
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Text = "Chart",
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                        Colour = Colours.GRAY_C
                                    },
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(0, 32),
                                        Children = new Drawable[]
                                        {
                                            new ChartSection()
                                        }
                                    }
                                }
                            },
                        }
                    }
                }
            }
        };
    }
}
