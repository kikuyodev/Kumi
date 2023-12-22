using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiSliderBar<T> : SliderBar<T>
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
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
    private readonly Circle background;

    private readonly SpriteText minText;
    private readonly SpriteText maxText;
    private readonly SpriteText valueText;

    public KumiSliderBar()
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
                    Height = 3,
                    Children = new Drawable[]
                    {
                        background = new Circle
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                        barColouredContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0,
                            Children = new Drawable[]
                            {
                                new Circle
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new Circle
                                {
                                    Size = new Vector2(16),
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.Centre,
                                }
                            }
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Margin = new MarginPadding { Top = 4 },
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

        valueText.Text = FormatValue(Convert.ToSingle(CurrentNumber.Value));
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
