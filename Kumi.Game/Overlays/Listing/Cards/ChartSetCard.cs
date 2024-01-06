using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Cards;

public partial class ChartSetCard : ClickableContainer
{
    private const float width = 330;
    private const float height = 100;

    private readonly APIChartSet chartSet;

    [Resolved]
    private Bindable<APIChartSet?> selectedChartSet { get; set; } = null!;

    public ChartSetCard(APIChartSet chartSet)
    {
        this.chartSet = chartSet;

        Masking = true;
        CornerRadius = 5;
        BorderThickness = 1.5f;
        BorderColour = Colours.GRAY_C.Opacity(0f);

        EdgeEffect = new EdgeEffectParameters
        {
            Hollow = true,
            Roundness = 5,
            Radius = 5,
            Colour = Colours.BLUE_ACCENT_LIGHTER.Opacity(0f),
            Type = EdgeEffectType.Glow
        };

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
                            new DrawableDifficultyBars(chartSet)
                            {
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                            },
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
        chartedFlow.AddText(chartSet.Creator.Username, c => c.Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 14));

        selectedChartSet.BindValueChanged(_ => updateState(), true);
    }

    private void updateState()
    {
        var isSelected = selectedChartSet.Value != null && selectedChartSet.Value.Id == chartSet.Id;

        FadeEdgeEffectTo(isSelected ? 0.2f : 0f, 200, Easing.OutQuint);
        this.TransformTo(nameof(BorderColour), (ColourInfo) Colours.GRAY_C.Opacity(isSelected ? 1f : 0f), 200, Easing.OutQuint);
    }
}
