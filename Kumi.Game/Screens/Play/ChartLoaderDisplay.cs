using Kumi.Game.Charts;
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

namespace Kumi.Game.Screens.Play;

public partial class ChartLoaderDisplay : CompositeDrawable
{
    private Container content = null!;
    private SpriteText loadingText = null!;
    private GlowingSpriteText readyText = null!;
    private DrawableChartInfo drawableChartInfo = null!;

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
                        drawableChartInfo = new DrawableChartInfo(chartInfo)
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
            drawableChartInfo.FadeOut(500, Easing.OutQuint);

            loadingText.FadeOut(500, Easing.OutQuint);
            readyText.FadeIn(500, Easing.OutQuint);
        }, true);
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
