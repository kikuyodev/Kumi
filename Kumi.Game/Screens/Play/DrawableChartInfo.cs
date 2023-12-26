using Kumi.Game.Charts;
using Kumi.Game.Charts.Drawables;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Play;

public partial class DrawableChartInfo : Container
{
    private readonly ChartInfo chartInfo;

    public DrawableChartInfo(ChartInfo chartInfo)
    {
        this.chartInfo = chartInfo;

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4),
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = new RomanisableString(chartInfo.Metadata.Title, chartInfo.Metadata.TitleRomanised),
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 20)
                    },
                    new SpriteText
                    {
                        Text = new RomanisableString(chartInfo.Metadata.Artist, chartInfo.Metadata.ArtistRomanised),
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Light)
                    },
                }
            },
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(4, 0),
                        AutoSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                AutoSizeAxes = Axes.X,
                                RelativeSizeAxes = Axes.Y,
                                Children = new Drawable[]
                                {
                                    new InfoDifficultyPill(0.0f)
                                    {
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                    },
                                }
                            },
                            new SpriteText
                            {
                                Text = chartInfo.DifficultyName,
                                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 20)
                            },
                        }
                    },
                    new SpriteText
                    {
                        Text = chartInfo.ChartSet?.Creator?.Username ?? "Unknown creator",
                        Font = KumiFonts.GetFont(FontFamily.Montserrat),
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight
                    },
                }
            },
        };
    }
    
    private partial class InfoDifficultyPill : DifficultyPill
    {
        public InfoDifficultyPill(float difficulty)
            : base(difficulty)
        {
        }

        protected override Drawable CreateContent()
            => new FillFlowContainer
            {
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(4, 0),
                Colour = Colours.Gray(0.1f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Vertical = 1,
                    Horizontal = 8
                },
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Star,
                        Size = new Vector2(12),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    new SpriteText
                    {
                        Text = Difficulty.ToString("N2"),
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            };
    }
}
