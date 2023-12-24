using Kumi.Game.Gameplay.Mods;
using Kumi.Game.Graphics;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Select.Mods;

public partial class DrawableModSelector : ClickableContainer, IFilterable, IStateful<bool>
{
    public event Action<bool>? StateChanged;

    public Action<Mod>? RequestSelection;

    public bool State
    {
        get => selected.Value;
        set
        {
            if (selected.Value == value) return;

            selected.Value = value;

            StateChanged?.Invoke(value);
        }
    }

    private readonly BindableBool selected = new BindableBool();

    private readonly Mod mod;

    public DrawableModSelector(Mod mod)
    {
        this.mod = mod;

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        
        Action = () => RequestSelection?.Invoke(mod);
    }

    private Box background = null!;
    private Box iconBackground = null!;
    private SpriteIcon icon = null!;
    private SpriteText name = null!;
    private SpriteText description = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.07f)
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(8, 0),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 48,
                            Masking = true,
                            CornerRadius = 5,
                            Children = new Drawable[]
                            {
                                iconBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colours.Gray(0.1f)
                                },
                                icon = new SpriteIcon
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Icon = mod.Icon,
                                    Colour = Colours.GRAY_C,
                                    Size = new Vector2(24),
                                    FillMode = FillMode.Fit
                                }
                            }
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 2),
                            Padding = new MarginPadding(4),
                            Children = new Drawable[]
                            {
                                name = new SpriteText
                                {
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 14),
                                    Text = mod.Name,
                                    Colour = Colours.GRAY_C
                                },
                                description = new SpriteText
                                {
                                    Font = KumiFonts.GetFont(size: 12),
                                    Text = mod.Description,
                                    Colour = Colours.GRAY_6
                                }
                            }
                        }
                    }
                }
            }
        };

        selected.BindValueChanged(_ => Schedule(selectedChanged), true);
    }

    private void selectedChanged()
    {
        background.FadeColour(State ? Colours.BLUE : Colours.Gray(0.07f), 100, Easing.OutQuint);
        iconBackground.FadeColour(State ? Colours.BLUE_LIGHT : Colours.Gray(0.1f), 100, Easing.OutQuint);
        icon.FadeColour(State ? Colours.GRAY_F : Colours.GRAY_C, 100, Easing.OutQuint);
        name.FadeColour(State ? Colours.GRAY_F : Colours.GRAY_C, 100, Easing.OutQuint);
        description.FadeColour(State ? Colours.GRAY_F : Colours.GRAY_6, 100, Easing.OutQuint);
    }

    public IEnumerable<LocalisableString> FilterTerms => new[] { mod.Name, mod.Description, mod.Acronym };

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
}
