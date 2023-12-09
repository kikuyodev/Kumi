using Kumi.Game.Graphics;
using Kumi.Game.Screens.Play.HUD.Health;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Play.HUD;

public partial class HealthDisplay : CompositeDrawable, IHasCurrentValue<double>
{
    public const int MAX_BINS = 50;

    private BindableDouble current = new BindableDouble
    {
        MinValue = 0d,
        MaxValue = 1d,
        Default = 0d
    };

    public Bindable<double> Current
    {
        get => current;
        set => current = new BindableDouble(value.Value)
        {
            MinValue = 0d,
            MaxValue = 1d,
            Default = value.Default
        };
    }

    private HealthBinRange normalRange = null!;
    private HealthBinRange clearRange = null!;

    public HealthDisplay()
    {
        AutoSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Spacing = new Vector2(2f, 0f),
            Children = new Drawable[]
            {
                normalRange = new HealthBinRange(0.7f)
                {
                    Height = 24f
                },
                clearRange = new HealthBinRange(1f, 0.7f)
                {
                    Height = 32f,
                    BarColour = Color4Extensions.FromHex("FFF9E5"),
                    GlowColour = Color4Extensions.FromHex("FFC800")
                }
            }
        };

        normalRange.Current.BindTo(Current);
        clearRange.Current.BindTo(Current);
    }

    private partial class HealthBinRange : Container
    {
        private readonly BindableDouble current = new BindableDouble();
        private readonly BindableDouble animatedCurrent = new BindableDouble();

        public IBindableNumber<double> Current => current;

        public Color4? BarColour;
        public Color4? GlowColour;

        private FillFlowContainer binContainer = null!;

        private int binCount;
        private HealthBin[] bins = null!;

        public HealthBinRange(float range, float offset = 0)
        {
            current.MinValue = offset;
            current.MaxValue = range;

            AutoSizeAxes = Axes.X;
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;

            calculateBinCount();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Shear = new Vector2(10f * MathF.PI / 180f, 0);
            Masking = true;
            CornerRadius = 5f;

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f),
                    Alpha = 0.98f
                },
                binContainer = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(4f, 0f),
                    Padding = new MarginPadding
                    {
                        Horizontal = 8,
                        Vertical = 4f
                    }
                }
            };

            generateBins();

            current.BindValueChanged(v => this.TransformBindableTo(animatedCurrent, v.NewValue, 250, Easing.OutQuint));
        }

        private void calculateBinCount()
        {
            // calculate bin count based on the range, MaxValue and MinValue are from a range of 0 to 1, so we'll have to multiply it by MAX_BINS
            binCount = (int) MathF.Ceiling((float) ((current.MaxValue - current.MinValue) * MAX_BINS));
        }

        private void generateBins()
        {
            bins = new HealthBin[binCount];

            for (var i = 0; i < binCount; i++)
            {
                bins[i] = new HealthBin
                {
                    Current = { BindTarget = animatedCurrent },
                    MinValue = current.MinValue + (current.MaxValue - current.MinValue) / binCount * i,
                    MaxValue = current.MinValue + (current.MaxValue - current.MinValue) / binCount * (i + 1)
                };

                if (BarColour != null)
                    bins[i].BarColour = BarColour.Value;
                if (GlowColour != null)
                    bins[i].GlowColour = GlowColour.Value;

                binContainer.Add(bins[i]);
            }
        }
    }
}
