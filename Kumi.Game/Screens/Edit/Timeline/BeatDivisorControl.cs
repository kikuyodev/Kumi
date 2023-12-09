using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class BeatDivisorControl : CompositeDrawable
{
    private SpriteText divisorText = null!;
    
    [Resolved]
    private BindableBeatDivisor beatDivisor { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 5;

        InternalChildren = new Drawable[]
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
                    new Dimension(),
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new DivisorTypeControl(),
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 4f),
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Padding = new MarginPadding { Right = 8 },
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = 18,
                                    Masking = true,
                                    CornerRadius = 5,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Depth = float.MaxValue,
                                            Colour = Colours.Gray(0.1f)
                                        },
                                        new TickSliderBar(beatDivisor)
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Padding = new MarginPadding { Horizontal = 5 }
                                        }
                                    }
                                },
                                divisorText = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 12),
                                    Colour = Colours.GRAY_6
                                }
                            }
                        }
                    },
                }
            }
        };
        
        beatDivisor.BindValueChanged(d => divisorText.Text = $"1/{d.NewValue}", true);
    }

    private partial class DivisorTypeControl : FillFlowContainer
    {
        [Resolved]
        private BindableBeatDivisor beatDivisor { get; set; } = null!;
        
        private readonly DivisorTextButton commonButton;
        private readonly DivisorTextButton tripletsButton;
        
        public DivisorTypeControl()
        {
            Direction = FillDirection.Vertical;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            Spacing = new Vector2(0, 1f);
            Padding = new MarginPadding { Left = 8 };
            
            Add(commonButton = new DivisorTextButton
            {
                Text = "common",
                Selected = true,
                Action = () => beatDivisor.SetArbitraryDivisor(4)
            });
            
            Add(tripletsButton = new DivisorTextButton
            {
                Text = "triplets",
                Action = () => beatDivisor.SetArbitraryDivisor(6)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            beatDivisor.ValidDivisors.BindValueChanged(_ => updateButtons(), true);
        }
        
        private void updateButtons()
        {
            commonButton.Selected = beatDivisor.ValidDivisors.Value == BeatDivisorCollection.COMMON;
            tripletsButton.Selected = beatDivisor.ValidDivisors.Value == BeatDivisorCollection.TRIPLETS;
        }

        private partial class DivisorTextButton : Button, IHasText
        {
            private readonly SpriteText text;
            private readonly SpriteText boldText;

            public LocalisableString Text
            {
                get => text.Text;
                set
                {
                    text.Text = value;
                    boldText.Text = value;
                }
            }

            private bool selected;
            
            public bool Selected
            {
                get => selected;
                set
                {
                    if (value == selected) return;

                    selected = value;
                    updateAlpha();
                }
            }
            
            public DivisorTextButton()
            {
                AutoSizeAxes = Axes.Both;
                Children = new Drawable[]
                {
                    text = new SpriteText
                    {
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 12),
                        Colour = Colours.GRAY_6
                    },
                    boldText = new SpriteText
                    {
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, weight: FontWeight.SemiBold, size: 12),
                        Colour = Colours.GRAY_C,
                        Alpha = 0
                    }
                };
            }
            
            private void updateAlpha()
            {
                ClearTransforms();
                
                text.FadeTo(selected ? 0 : 1, 100, Easing.OutQuint);
                boldText.FadeTo(selected ? 1 : 0, 100, Easing.OutQuint);
            }
        }
    }

    private partial class TickSliderBar : SliderBar<int>
    {
        private readonly BindableBeatDivisor beatDivisor;

        public TickSliderBar(BindableBeatDivisor beatDivisor)
        {
            CurrentNumber.BindTo(this.beatDivisor = beatDivisor);

            RangePadding = 5;
            Padding = new MarginPadding { Horizontal = RangePadding };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatDivisor.ValidDivisors.BindValueChanged(_ => updateDivisors(), true);
        }

        private void updateDivisors()
        {
            ClearInternal();
            CurrentNumber.ValueChanged -= updateSelection;

            foreach (var divisor in beatDivisor.ValidDivisors.Value.Divisors)
            {
                AddInternal(new Tick(divisor)
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.Centre,
                    RelativePositionAxes = Axes.Both,
                    Colour = BindableBeatDivisor.GetColourFor(divisor),
                    X = getMappedPosition(divisor)
                });
            }

            CurrentNumber.ValueChanged += updateSelection;
            CurrentNumber.TriggerChange();
        }

        private void updateSelection(ValueChangedEvent<int> divisor)
        {
            if (InternalChildren.FirstOrDefault(c => c is Tick { Selected: true }) is Tick old)
                old.Selected = false;

            if (InternalChildren.FirstOrDefault(c => c is Tick newTick && newTick.Divisor == divisor.NewValue) is Tick tick)
                tick.Selected = true;
        }

        protected override void UpdateValue(float value)
        {
        }

        public override bool HandleNonPositionalInput => IsHovered && !CurrentNumber.Disabled;

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    beatDivisor.SelectNext();
                    OnUserChange(Current.Value);
                    return true;

                case Key.Left:
                    beatDivisor.SelectPrevious();
                    OnUserChange(Current.Value);
                    return true;

                default:
                    return false;
            }
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            handleMouseInput(e.ScreenSpaceMousePosition);
            return base.OnMouseDown(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            handleMouseInput(e.ScreenSpaceMousePosition);
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            handleMouseInput(e.ScreenSpaceMousePosition);
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            handleMouseInput(e.ScreenSpaceMousePosition);
        }

        private void handleMouseInput(Vector2 spMousePosition)
        {
            var xPosition = (ToLocalSpace(spMousePosition).X - RangePadding) / UsableWidth;

            CurrentNumber.Value = beatDivisor.ValidDivisors.Value.Divisors.MinBy(d => Math.Abs(getMappedPosition(d) - xPosition));
            OnUserChange(Current.Value);
        }

        private float getMappedPosition(float divisor) => 1 - 1 / divisor;

        private partial class Tick : Circle
        {
            public readonly int Divisor;

            private bool selected;

            public bool Selected
            {
                get => selected;
                set
                {
                    if (value == selected) return;

                    selected = value;
                    updateAlpha();
                }
            }

            public Tick(int divisor)
            {
                Divisor = divisor;

                Size = new Vector2(getWidth(), selected ? 12f : 8f);
                updateAlpha();
            }

            private float getWidth()
            {
                switch (Divisor)
                {
                    case 1:
                        return 4f;

                    case 2:
                    case 3:
                    case 4:
                        return 3f;

                    default:
                        return 2f;
                }
            }

            private void updateAlpha()
            {
                ClearTransforms();

                this.FadeTo(selected ? 1 : 0.5f, 100, Easing.OutQuint);
                this.ResizeHeightTo(selected ? 12f : 8f, 100, Easing.OutQuint);
            }
        }
    }
}
