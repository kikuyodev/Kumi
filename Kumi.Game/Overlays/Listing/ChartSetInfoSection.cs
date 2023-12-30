using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Sprites;
using Kumi.Game.Online.API.Charts;
using Kumi.Game.Overlays.Listing.Info;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Listing;

public partial class ChartSetInfoSection : VisibilityContainer
{
    private const float height = 266;
    
    [Cached]
    private Bindable<APIChart?> selectedChart { get; set; } = new Bindable<APIChart?>();

    public ChartSetInfoSection()
    {
        RelativeSizeAxes = Axes.X;

        Masking = true;
        CornerRadius = 5;
    }

    private SpriteText difficultyName = null!;

    private SpriteText title = null!;
    private SpriteText artist = null!;
    
    private UpdateableAvatarSprite avatar = null!;
    private SpriteText creatorName = null!;
    private TextFlowContainer submittedOn = null!;

    [BackgroundDependencyLoader]
    private void load(Bindable<APIChartSet> selectedChartSet)
    {
        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Masking = true,
                Children = new Drawable[]
                {
                    new ChartInfoCover
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(12),
                        Padding = new MarginPadding
                        {
                            Horizontal = 28,
                            Vertical = 20
                        },
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(4),
                                Children = new Drawable[]
                                {
                                    new DifficultySelector
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Direction = FillDirection.Horizontal,
                                        Spacing = new Vector2(8, 0)
                                    },
                                    difficultyName = new SpriteText
                                    {
                                        Shadow = true,
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.Medium),
                                        Colour = Colours.GRAY_C
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                                Spacing = new Vector2(2),
                                Children = new Drawable[]
                                {
                                    title = new SpriteText
                                    {
                                        Shadow = true,
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 24),
                                        Colour = Colours.Gray(0.9f)
                                    },
                                    artist = new SpriteText
                                    {
                                        Shadow = true,
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 20),
                                        Colour = Colours.GRAY_C
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(4),
                                Children = new Drawable[]
                                {
                                    avatar = new UpdateableAvatarSprite
                                    {
                                        Size = new Vector2(48)
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 48,
                                        Children = new Drawable[]
                                        {
                                            new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                Direction = FillDirection.Vertical,
                                                Spacing = new Vector2(2),
                                                Children = new Drawable[]
                                                {
                                                    creatorName = new SpriteText
                                                    {
                                                        Shadow = true,
                                                        Font = KumiFonts.GetFont(weight: FontWeight.SemiBold),
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                        Colour = Colours.GRAY_C
                                                    },
                                                    submittedOn = new TextFlowContainer(c =>
                                                    {
                                                        c.Font = KumiFonts.GetFont(size: 14);
                                                        c.Colour = Colours.GRAY_C;
                                                    })
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                        Alpha = 0.5f
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Horizontal,
                                Spacing = new Vector2(4),
                                Children = new Drawable[]
                                {
                                    new PreviewButton(),
                                    new DownloadButton
                                    {
                                        RelativeSizeAxes = Axes.None,
                                        Width = 125,
                                        Height = 32,
                                        Important = true,
                                    }
                                }
                            },
                        }
                    },
                    new ChartStatistics
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        X = -28
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Horizontal = 28,
                    Vertical = 20
                },
                Children = new Drawable[]
                {
                    new SkewedChartStatus
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight
                    }
                }
            }
        };

        selectedChartSet.BindValueChanged(v =>
        {
            if (v.NewValue == null)
            {
                selectedChart.Value = null;
                return;
            }

            selectedChart.Value = v.NewValue.Charts.FirstOrDefault();
            updateDisplay();
        }, true);
    }

    private void updateDisplay()
    {
        difficultyName.Text = selectedChart.Value!.DifficultyName;

        title.Text = new RomanisableString(selectedChart.Value!.Title, selectedChart.Value!.Romanised?.TitleRomanised);
        artist.Text = new RomanisableString(selectedChart.Value!.Artist, selectedChart.Value!.Romanised?.ArtistRomanised);
        
        var creator = selectedChart.Value!.Creators.FirstOrDefault();
        avatar.Account = creator;
        creatorName.Text = creator?.Username ?? "Unknown";
        
        submittedOn.Clear();
        submittedOn.AddText("Submitted on ");
        submittedOn.AddText(selectedChart.Value!.CreatedAt.ToString("dd MMM yyyy"), c => { c.Font = KumiFonts.GetFont(weight: FontWeight.SemiBold, size: 14); });
    }

    protected override void PopIn()
    {
        this.ResizeHeightTo(height, 200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.ResizeHeightTo(0, 200, Easing.OutQuint);
    }
}
