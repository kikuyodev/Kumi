using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Select.List;

public partial class ChartSetListItem : CompositeDrawable
{
    public readonly ChartSetInfo ChartSetInfo;
    public readonly BindableBool Selected = new BindableBool();

    public Action? RequestSelect;

    private Box highlight = null!;
    private Container content = null!;

    public ChartSetListItem(ChartSetInfo chartSetInfo)
    {
        ChartSetInfo = chartSetInfo;
    }

    [BackgroundDependencyLoader]
    private void load(ChartManager manager)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        DelayedLoadWrapper background;

        InternalChildren = new Drawable[]
        {
            content = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                CornerRadius = 5,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Colour = Colours.CYAN_ACCENT_LIGHT.Opacity(0f),
                    Type = EdgeEffectType.Glow,
                    Radius = 12,
                    Roundness = 5,
                },
                Children = new Drawable[]
                {
                    background = new DelayedLoadWrapper(() => new ListItemBackground(manager.GetWorkingChart(ChartSetInfo.Charts.First()))
                    {
                        RelativeSizeAxes = Axes.Both
                    }, 200)
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    highlight = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0f,
                        AlwaysPresent = true,
                        Colour = ColourInfo.GradientHorizontal(Colours.CYAN_ACCENT_LIGHT, Colours.PURPLE_ACCENT_LIGHT)
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding
                        {
                            Horizontal = 16,
                            Vertical = 12
                        },
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 4),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Text = new RomanisableString(ChartSetInfo.Metadata.Title, ChartSetInfo.Metadata.TitleRomanised),
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold)
                                    },
                                    new SpriteText
                                    {
                                        Text = new RomanisableString(ChartSetInfo.Metadata.Artist, ChartSetInfo.Metadata.ArtistRomanised),
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 12)
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(0, 4),
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Children = new Drawable[]
                                {
                                    // TODO: Rank status and difficulties
                                }
                            }
                        }
                    }
                }
            }
        };

        background.DelayedLoadComplete += d => d.FadeInFromZero(200);

        Selected.BindValueChanged(onSelectionChanged, true);
    }

    protected override bool OnClick(ClickEvent e)
    {
        RequestSelect?.Invoke();
        return base.OnClick(e);
    }

    private void onSelectionChanged(ValueChangedEvent<bool> val)
    {
        if (val.NewValue)
        {
            highlight.FadeTo(0.15f, 200, Easing.OutQuint);
            content.FadeEdgeEffectTo(Colours.CYAN_ACCENT_LIGHT.Opacity(0.25f), 200, Easing.OutQuint);
        }
        else
        {
            highlight.FadeOut(200, Easing.OutQuint);
            content.FadeEdgeEffectTo(Colours.CYAN_ACCENT_LIGHT.Opacity(0), 200, Easing.OutQuint);
        }
    }
}
