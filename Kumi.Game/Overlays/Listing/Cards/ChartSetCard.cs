using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Listing.Cards;

public partial class ChartSetCard : ClickableContainer
{
    private const float width = 400;
    private const float height = 100;

    private readonly APIChartSet chartSet;

    public ChartSetCard(APIChartSet chartSet)
    {
        this.chartSet = chartSet;

        Masking = true;
        CornerRadius = 5;

        Width = width;
        Height = height;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        TextFlowContainer chartedFlow;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.06f)
            },
            new SetCardCover(chartSet)
            {
                RelativeSizeAxes = Axes.Both
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(8),
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 2),
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = new RomanisableString(chartSet.Title, chartSet.Romanised?.TitleRomanised),
                                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                Colour = Colours.GRAY_C
                            },
                            new SpriteText
                            {
                                Text = new RomanisableString(chartSet.Artist, chartSet.Romanised?.ArtistRomanised),
                                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 14),
                                Colour = Colours.GRAY_6
                            },
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 2),
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Children = new Drawable[]
                        {
                            chartedFlow = new TextFlowContainer(c =>
                            {
                                c.Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 14);
                                c.Colour = Colours.GRAY_6;
                            })
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 2),
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Children = new Drawable[]
                        {
                            new DrawableSetStatus(chartSet.Status)
                            {
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.BottomRight,
                            }
                        }
                    }
                }
            }
        };

        chartedFlow.AddText("Charted by ");
        chartedFlow.AddText(chartSet.Creator.Username);
    }

    private partial class DrawableSetStatus : CompositeDrawable
    {
        private readonly APIChartSetStatus status;

        public DrawableSetStatus(APIChartSetStatus status)
        {
            this.status = status;

            AutoSizeAxes = Axes.Both;
            Masking = true;
            CornerRadius = 3;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = getBackgroundColour()
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                    Colour = getForegroundColour(),
                    Text = status.GetDescription().ToUpperInvariant(),
                    UseFullGlyphHeight = false,
                    Padding = new MarginPadding(4)
                }
            };
        }

        private Color4 getBackgroundColour()
            => status switch
            {
                APIChartSetStatus.Ranked => Color4Extensions.FromHex("39AC73"),
                APIChartSetStatus.Pending => Color4Extensions.FromHex("AC7339"),
                APIChartSetStatus.WorkInProgress => Color4Extensions.FromHex("AC4339"),
                APIChartSetStatus.Qualified => Color4Extensions.FromHex("3986AC"),
                APIChartSetStatus.Graveyard => Color4Extensions.FromHex("161B1D"),
                _ => Colours.GRAY_2
            };

        private Color4 getForegroundColour()
            => status switch
            {
                APIChartSetStatus.Graveyard => Color4Extensions.FromHex("576E75"),
                _ => Colours.GRAY_C
            };
    }
}
