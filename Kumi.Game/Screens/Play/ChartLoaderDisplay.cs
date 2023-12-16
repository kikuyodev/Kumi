using Kumi.Game.Charts;
using Kumi.Game.Charts.Drawables;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Play;

public partial class ChartLoaderDisplay : CompositeDrawable
{
    private Container content = null!;
    private FillFlowContainer songFlow = null!;
    private FillFlowContainer chartFlow = null!;
    private SpriteText loadingText = null!;
    private GlowingSpriteText readyText = null!;

    public readonly BindableBool IsReady = new BindableBool();
    
    private readonly ChartInfo chartInfo;
    
    public ChartLoaderDisplay(ChartInfo chartInfo)
    {
        this.chartInfo = chartInfo;
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Width = 0.9f,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new LoaderBar
                {
                    IsReady = { BindTarget = IsReady },
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Y = 12
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Y = 24,
                    Children = new Drawable[]
                    {
                        songFlow = new FillFlowContainer
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
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 24)
                                },
                                new SpriteText
                                {
                                    Text = new RomanisableString(chartInfo.Metadata.Artist, chartInfo.Metadata.ArtistRomanised),
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Light, 20)
                                },
                            }
                        },
                        loadingText = new SpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = "LOADING",
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Light, 24),
                        },
                        readyText = new GlowingSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = "READY!",
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Light, 24),
                            BlurredFont = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Bold, 24),
                            GlowColour = Colours.BLUE_ACCENT.Opacity(0.9f),
                            Alpha = 0,
                            AlwaysPresent = true,
                        },
                        chartFlow = new FillFlowContainer
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
                                                new LoaderDifficultyPill(0.0f)
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
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
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Light),
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight
                                },
                            }
                        },
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        IsReady.BindValueChanged(v =>
        {
            if (!v.NewValue)
                return;

            content.ResizeWidthTo(1.1f, 500, Easing.OutQuint);
            songFlow.FadeOut(500, Easing.OutQuint);
            chartFlow.FadeOut(500, Easing.OutQuint);

            loadingText.FadeOut(500, Easing.OutQuint);
            readyText.FadeIn(500, Easing.OutQuint);
        }, true);
    }
    
    private partial class LoaderDifficultyPill : DifficultyPill
    {
        public LoaderDifficultyPill(float difficulty)
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

    private partial class LoaderBar : Container
    {
        private const float animation_length = 2000;
        
        private Circle bar1 = null!;
        private Circle bar2 = null!;

        public readonly BindableBool IsReady = new BindableBool();
        
        public LoaderBar()
        {
            RelativeSizeAxes = Axes.X;
            Height = 6;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.25f,
                    Colour = Color4Extensions.FromHex("CCEEFF")
                },
                bar1 = createBar(),
                bar2 = createBar(),
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bar1
               .MoveToX(0f)
               .FadeTo(1f, animation_length / 4, Easing.InQuint)
               .MoveToX(0.5f, animation_length / 2, Easing.InQuint)
               .ResizeWidthTo(50, animation_length / 2, Easing.InQuint)
               .Then()
               .MoveToX(1f, animation_length / 2, Easing.OutQuint)
               .ResizeWidthTo(0, animation_length / 2, Easing.OutQuint)
               .FadeTo(0f, animation_length / 4, Easing.OutQuint)
               .Loop();

            bar2.Delay(animation_length / 4)
               .MoveToX(0f)
               .FadeTo(1f, animation_length / 4, Easing.InQuint)
               .MoveToX(0.5f, animation_length / 2, Easing.InQuint)
               .ResizeWidthTo(50, animation_length / 2, Easing.InQuint)
               .Then()
               .MoveToX(1f, animation_length / 2, Easing.OutQuint)
               .ResizeWidthTo(0, animation_length / 2, Easing.OutQuint)
               .FadeTo(0f, animation_length / 4, Easing.OutQuint)
               .Loop();
            
            IsReady.BindValueChanged(v =>
            {
                if (!v.NewValue)
                    return;

                bar1.ClearTransforms();
                bar2.ClearTransforms();
                bar1.ResizeWidthTo(0);
                bar2.ResizeWidthTo(0);

                bar1.Anchor = Anchor.Centre;
                bar1.Origin = Anchor.Centre;
                bar1.RelativeSizeAxes = Axes.Both;
                bar1.FadeTo(1f, 500, Easing.OutQuint);
                bar1.MoveToX(0f)
                   .ResizeWidthTo(0f)
                   .ResizeWidthTo(1f, 500, Easing.OutQuint);
            }, true);
        }

        private Circle createBar()
            => new Circle
            {
                RelativeSizeAxes = Axes.Y,
                RelativePositionAxes = Axes.X,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Colour = Color4Extensions.FromHex("CCEEFF"),
                Alpha = 0,
                AlwaysPresent = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Colour = Colours.BLUE_ACCENT.Opacity(0.25f),
                    Roundness = 6,
                    Radius = 8,
                    Type = EdgeEffectType.Glow
                }
            };
    }
}
