using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Settings.Components;

public abstract partial class LabelledSettingItem<T> : CompositeDrawable, IConditionalFilterable, IHasCurrentValue<T>
{
    protected abstract Drawable CreateControl();

    protected Drawable Control { get; }

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

    private SpriteText? descriptionText;

    private LocalisableString description;

    public LocalisableString Description
    {
        get => description;
        set
        {
            description = value;

            if (descriptionText == null)
            {
                descriptionText = new SpriteText
                {
                    Font = KumiFonts.GetFont(size: 12),
                    Colour = Colours.GRAY_6
                };

                labelFlow.Add(descriptionText);
            }

            descriptionText.Text = value;
        }
    }

    private readonly FillFlowContainer labelFlow;

    protected LabelledSettingItem()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        InternalChildren = new[]
        {
            labelFlow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4),
                Children = new Drawable[]
                {
                    labelText = new SpriteText
                    {
                        Font = KumiFonts.GetFont(size: 12),
                        Colour = Colours.GRAY_C
                    },
                }
            },
            Control = CreateControl().With(d =>
            {
                d.Anchor = Anchor.TopRight;
                d.Origin = Anchor.TopRight;
            })
        };
    }

    public IEnumerable<LocalisableString> FilterTerms => new[] { label, description };

    private bool matchingFilter = true;

    public bool MatchingFilter
    {
        get => matchingFilter;
        set
        {
            var wasPresent = IsPresent;

            matchingFilter = value;

            if (wasPresent != IsPresent)
                Invalidate(Invalidation.Presence);
        }
    }

    public override bool IsPresent => base.IsPresent && MatchingFilter;

    public bool FilteringActive { get; set; }

    public BindableBool CanBeShown { get; set; } = new BindableBool(true);
    IBindable<bool> IConditionalFilterable.CanBeShown => CanBeShown;
}
