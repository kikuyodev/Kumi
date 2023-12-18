using Kumi.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Screens.Edit.Components;

public partial class EditorSliderBar<T> : SliderBar<T>
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public float BarHeight
    {
        get => barContainer.Height;
        set => barContainer.Height = value;
    }

    public ColourInfo BarColour
    {
        get => barColouredContainer.Colour;
        set => barColouredContainer.Colour = value;
    }

    public ColourInfo BackgroundColour
    {
        get => background.Colour;
        set => background.Colour = value;
    }

    private readonly Container barContainer;
    private readonly Container barColouredContainer;
    private readonly Box background;

    private readonly SpriteText minText;
    private readonly SpriteText maxText;
    private readonly SpriteText valueText;

    public EditorSliderBar()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 4),
            Children = new Drawable[]
            {
                barContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Masking = true,
                    CornerRadius = 5,
                    Children = new Drawable[]
                    {
                        background = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                        barColouredContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0,
                            Masking = true,
                            CornerRadius = 5,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Alpha = 0.25f
                                },
                                new Circle
                                {
                                    RelativeSizeAxes = Axes.Y,
                                    Width = 6,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                }
                            }
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        minText = new SpriteText
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Font = KumiFonts.GetFont(size: 12),
                            Colour = Colours.GRAY_6,
                            Text = FormatValue(Convert.ToSingle(CurrentNumber.MinValue))
                        },
                        maxText = new SpriteText
                        {
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Font = KumiFonts.GetFont(size: 12),
                            Colour = Colours.GRAY_6,
                            Text = FormatValue(Convert.ToSingle(CurrentNumber.MaxValue))
                        },
                        valueText = new SpriteText
                        {
                            Origin = Anchor.TopCentre,
                            RelativePositionAxes = Axes.X,
                            Font = KumiFonts.GetFont(size: 12),
                            Colour = Colours.GRAY_C
                        }
                    }
                }
            }
        };

        CurrentNumber.MinValueChanged += _ => minText.Text = FormatValue(Convert.ToSingle(CurrentNumber.MinValue));
        CurrentNumber.MaxValueChanged += _ => maxText.Text = FormatValue(Convert.ToSingle(CurrentNumber.MaxValue));
    }

    protected virtual string FormatValue(float value) => $"{value:N2}";

    protected override void UpdateValue(float value)
    {
        barColouredContainer.ResizeWidthTo(NormalizedValue, 200, Easing.OutQuint);

        valueText.Text = FormatValue(value);
        valueText.MoveToX(NormalizedValue, 200, Easing.OutQuint);
    }

    protected override void Update()
    {
        base.Update();
        
        // check for overlaps, and if so, hide the min-max text
        if (valueText.ScreenSpaceDrawQuad.AABBFloat.IntersectsWith(minText.ScreenSpaceDrawQuad.AABBFloat))
            minText.Hide();
        else
            minText.Show();
        
        if (valueText.ScreenSpaceDrawQuad.AABBFloat.IntersectsWith(maxText.ScreenSpaceDrawQuad.AABBFloat))
            maxText.Hide();
        else
            maxText.Show();
    }
}
