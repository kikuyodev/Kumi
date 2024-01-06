using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class DifficultySelector : FillFlowContainer
{
    [BackgroundDependencyLoader]
    private void load(Bindable<APIChartSet?> selectedChartSet, Bindable<APIChart?> selectedChart)
    {
        selectedChartSet.BindValueChanged(v =>
        {
            Clear();
            
            if (v.NewValue == null)
                return;
            
            AddRange(v.NewValue.Charts.Select(c => new DifficultyItem(c)
            {
                Action = () => selectedChart.Value = c
            }));
        }, true);
    }
    
    private partial class DifficultyItem : ClickableContainer
    {
        private const float height = 20;
        
        private Box paddingLeft = null!;
        private Box paddingRight = null!;

        private Container background = null!;
        private SpriteText text = null!;
        private SpriteText textBold = null!;
        
        private readonly APIChart chart;

        [Resolved]
        private Bindable<APIChart?> selectedChart { get; set; } = null!;

        public DifficultyItem(APIChart chart)
        {
            this.chart = chart;
            
            AutoSizeAxes = Axes.X;
            Height = 24;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                background = new Container
                {
                    Masking = true,
                    CornerRadius = 3,
                    RelativeSizeAxes = Axes.X,
                    Height = 3,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = getBackgroundColour()
                        }
                    }
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        paddingLeft = new Box
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Y,
                            Width = 0,
                            Alpha = 0,
                            AlwaysPresent = true,
                        },
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[]
                            {
                                text = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium, 14),
                                    UseFullGlyphHeight = false,
                                    Colour = Colours.GRAY_C,
                                    Text = chart.Difficulty.Difficulty.ToString("N2")
                                },
                                textBold = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Bold, 14),
                                    UseFullGlyphHeight = false,
                                    Colour = getForegroundColour(),
                                    Text = chart.Difficulty.Difficulty.ToString("N2")
                                },
                            }
                        },
                        paddingRight = new Box
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Y,
                            Width = 0,
                            Alpha = 0,
                            AlwaysPresent = true,
                        }
                    }
                }
            };
            
            selectedChart.BindValueChanged(_ => updateState(), true);
        }

        private void updateState()
        {
            var selected = selectedChart.Value != null && selectedChart.Value.Id == chart.Id;

            const float padding_width = 4;
            
            background.ResizeHeightTo(selected ? height : 3, 200, Easing.OutQuint);
            background.MoveToY(selected ? 0 : 3, 200, Easing.OutQuint);
            paddingLeft.ResizeWidthTo(selected ? padding_width : 0, 200, Easing.OutQuint);
            paddingRight.ResizeWidthTo(selected ? padding_width : 0, 200, Easing.OutQuint);
            
            text.FadeTo(selected ? 0 : 1, 200, Easing.OutQuint);
            textBold.FadeTo(selected ? 1 : 0, 200, Easing.OutQuint);
        }

        private Colour4 getBackgroundColour()
            => Colours.GRAY_C;

        private Colour4 getForegroundColour()
            => Colours.GRAY_2;
    }
}
