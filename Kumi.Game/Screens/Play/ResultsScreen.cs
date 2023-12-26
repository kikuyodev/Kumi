using Kumi.Game.Charts;
using Kumi.Game.Charts.Drawables;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Sprites;
using Kumi.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Play;

public partial class ResultsScreen : ScreenWithChartBackground
{
    public override float ParallaxAmount => 0.0025f;
    public override float DimAmount => 0.45f;
    public override float BlurAmount => 10f;

    private readonly ScoreInfo score;

    public ResultsScreen(ScoreInfo score)
    {
        this.score = score;
    }

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> chart)
    {
        AddInternal(new Container
        {
            RelativeSizeAxes = Axes.Both,
            Width = 0.5f,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Padding = new MarginPadding(32),
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 24),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Name = "Ranking",
                            Children = new Drawable[]
                            {
                                score.Failed
                                    ? new SpriteIcon
                                    {
                                        Icon = FontAwesome.Solid.Times,
                                        Size = new Vector2(32),
                                        Colour = Colour4.Red
                                    }
                                    : new SpriteIcon
                                    {
                                        Icon = FontAwesome.Solid.Check,
                                        Size = new Vector2(32),
                                        Colour = Colour4.Green
                                    },
                                new SpriteText
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    Text = score.ScoreRank.GetDescription() + " " + score.ComboRank.GetDescription(),
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 24)
                                }
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 80,
                            Masking = true,
                            CornerRadius = 5,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Name = "Chart background",
                            Child = new ChartBackgroundSprite(chart.Value)
                            {
                                RelativeSizeAxes = Axes.Both,
                                FillMode = FillMode.Fill,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            }
                        },
                        new DrawableChartInfo(chart.Value.ChartInfo)
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Name = "Chart info",
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 40,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Name = "Score info",
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(8, 0),
                                    Children = new Drawable[]
                                    {
                                        // Probably not a good idea to use an updateable avatar sprite here but it's the only
                                        // one available with a corner radius, so it doesn't matter too much.
                                        new UpdateableAvatarSprite(score.Account)
                                        {
                                            Size = new Vector2(40),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(0, 2),
                                            Children = new Drawable[]
                                            {
                                                new SpriteText
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Text = score.Account.Username,
                                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                                    Colour = Colours.GRAY_C
                                                },
                                                new SpriteText
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Text = "Played on " + score.Date.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
                                                    Font = KumiFonts.GetFont(size: 12),
                                                    Colour = Colours.GRAY_C,
                                                    Alpha = 0.5f
                                                },
                                            }
                                        }
                                    }
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(12, 0),
                                    Children = new[]
                                    {
                                        createScoreStatistic("Score", score.TotalScore.ToString("N0")),
                                        createScoreStatistic("Max Combo", score.MaxCombo.ToString("N0") + "x"),
                                    }
                                }
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            // Judgement statistics has a horizontal padding of -4, so we're adding this to make sure the very edges
                            // line up with the rest of the contents.
                            Padding = new MarginPadding { Horizontal = -4 },
                            Name = "Score statistics",
                            Child = new GridContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                ColumnDimensions = new[]
                                {
                                    new Dimension(),
                                    new Dimension(),
                                    new Dimension(),
                                    new Dimension(),
                                },
                                RowDimensions = new[]
                                {
                                    new Dimension(GridSizeMode.AutoSize)
                                },
                                Content = new[]
                                {
                                    new[]
                                    {
                                        createJudgementStatistic(Colours.RED_ACCENT_LIGHT, "Miss", score.Statistics.Miss),
                                        createJudgementStatistic(Colours.YELLOW_ACCENT_LIGHT, "Bad", score.Statistics.Bad),
                                        createJudgementStatistic(Colours.SEAFOAM_ACCENT_LIGHT, "Ok", score.Statistics.Ok),
                                        createJudgementStatistic(Colours.BLUE_ACCENT_LIGHT, "Good", score.Statistics.Good),
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
    }

    private Drawable createScoreStatistic(LocalisableString label, string value)
        => new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 2),
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Text = label,
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12),
                    Alpha = 0.5f,
                },
                new SpriteText
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Font = KumiFonts.GetFont(FontFamily.Montserrat),
                    Text = value
                }
            }
        };

    private Drawable createJudgementStatistic(Color4 accent, LocalisableString label, int value)
        => new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight,
            Spacing = new Vector2(0, 4),
            Padding = new MarginPadding { Horizontal = 4 },
            Children = new Drawable[]
            {
                new Circle
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 2,
                    Colour = accent,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = label,
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12),
                            Alpha = 0.5f,
                        },
                        new SpriteText
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Text = value.ToString("N0") + "x",
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 12),
                        }
                    }
                }
            }
        };
}
