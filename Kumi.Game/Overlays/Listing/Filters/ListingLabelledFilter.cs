using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Filters;

public abstract partial class ListingLabelledFilter<T> : CompositeDrawable, IHasCurrentValue<T>
{
    protected abstract Drawable CreateControl();

    protected Drawable Control { get; }

    protected virtual bool ShowLabel => true;

    public override bool HandlePositionalInput => Current.Disabled;
    public override bool HandleNonPositionalInput => Current.Disabled;

    private IHasCurrentValue<T>? controlWithCurrent => Control as IHasCurrentValue<T>;

    public Bindable<T> Current
    {
        get => controlWithCurrent?.Current ?? throw new NullReferenceException("Control does not implement IHasCurrentValue<T>");
        set
        {
            if (controlWithCurrent != null)
                controlWithCurrent.Current = value;
            else
                throw new NullReferenceException("Control does not implement IHasCurrentValue<T>");
        }
    }

    private readonly SpriteText labelText;

    private LocalisableString label;

    public LocalisableString Label
    {
        get => label;
        set
        {
            label = value;
            labelText.Text = value;
        }
    }

    protected ListingLabelledFilter()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 4),
            Children = new[]
            {
                labelText = new SpriteText
                {
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 14),
                    Colour = Colours.GRAY_C,
                    Alpha = ShowLabel ? 1 : 0,
                },
                Control = CreateControl()
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Current.BindDisabledChanged(_ => updateDisabled());
    }

    private void updateDisabled()
    {
        this.FadeTo(Current.Disabled ? 0.5f : 1, 200, Easing.OutQuint);
    }
}
