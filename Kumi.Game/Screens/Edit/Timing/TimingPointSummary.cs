using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingPointSummary : ClickableContainer
{
    public const float HEIGHT = 30;
    
    private readonly TimingPoint point;

    [Resolved(name: "selected_point")]
    private Bindable<TimingPoint> selectedPoint { get; set; } = null!;

    [Resolved(name: "current_point")]
    private Bindable<TimingPoint> currentPoint { get; set; } = null!;

    public TimingPointSummary(TimingPoint point)
    {
        this.point = point;

        RelativeSizeAxes = Axes.X;
        Height = HEIGHT;
    }

    private Container markerContainer = null!;
    private Container mainContent = null!;
    private Box background = null!;

    private SpriteText timeText = null!;

    private SpriteText bpmText = null!;
    private SpriteText signatureText = null!;
    
    private SpriteText volumeText = null!;
    private SpriteText relativeText = null!;
    private SummarySliderBar<float> relativeSlider = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            markerContainer = new Container
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Rotation = 45,
                        Size = new Vector2(8),
                        Colour = Colours.RED_ACCENT,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.Centre
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 2,
                        Colour = Colours.RED_ACCENT,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight
                    }
                }
            },
            mainContent = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 12 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 5,
                    Children = new Drawable[]
                    {
                        background = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.1f),
                            Alpha = 0,
                            AlwaysPresent = true,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Horizontal = 12 },
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Spacing = new Vector2(20, 0),
                                    Direction = FillDirection.Horizontal,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Children = new Drawable[]
                                    {
                                        timeText = new SpriteText
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Font = KumiFonts.GetFont(size: 12),
                                            Colour = Colours.GRAY_6
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Spacing = new Vector2(8, 0),
                                            Direction = FillDirection.Horizontal,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Children = new Drawable[]
                                            {
                                                bpmText = new SpriteText
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, size: 12),
                                                    Colour = Colours.YELLOW_ACCENT_LIGHT
                                                },
                                                signatureText = new SpriteText
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, size: 12),
                                                    Colour = Colours.YELLOW_ACCENT_LIGHT
                                                }
                                            }
                                        }
                                    }
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Spacing = new Vector2(20, 0),
                                    Direction = FillDirection.Horizontal,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Children = new Drawable[]
                                    {
                                        volumeText = new SpriteText
                                        {
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, size: 12),
                                            Colour = Colours.RED_ACCENT_LIGHT
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Spacing = new Vector2(8, 0),
                                            Direction = FillDirection.Horizontal,
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            Children = new Drawable[]
                                            {
                                                relativeText = new SpriteText
                                                {
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, size: 12),
                                                    Colour = Colours.BLUE_ACCENT_LIGHT
                                                },
                                                relativeSlider = new SummarySliderBar<float>
                                                {
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Height = 8,
                                                    Width = 100,
                                                    ForegroundColour = Colours.BLUE_ACCENT_LIGHT,
                                                    Current = new BindableFloat
                                                    {
                                                        Value = point.RelativeScrollSpeed,
                                                        MinValue = 0.5f,
                                                        MaxValue = 2.5f,
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        currentPoint.BindValueChanged(_ => updateMarker(), true);
        selectedPoint.BindValueChanged(_ => updateBackground(), true);

        point.StartTimeBindable.BindValueChanged(_ => updateContents(), true);
        point.RelativeScrollSpeedBindable.BindValueChanged(_ => updateContents(), true);
        point.VolumeBindable.BindValueChanged(_ => updateContents(), true);
        point.PointTypeBindable.BindValueChanged(_ => updateContents(), true);
        point.FlagsBindable.BindValueChanged(_ => updateContents(), true);
    }

    protected override void LoadComplete()
    {
        relativeSlider.Current.Disabled = true;
    }

    private void updateMarker()
    {
        markerContainer.FadeTo(ReferenceEquals(currentPoint.Value, point) ? 1 : 0, 200, Easing.OutQuint);
    }

    private void updateBackground()
    {
        background.FadeTo(ReferenceEquals(selectedPoint.Value, point) ? 1 : 0, 200, Easing.OutQuint);
    }

    private void updateContents()
    {
        var timestamp = TimeSpan.FromMilliseconds(point.StartTime);
        timeText.Text = $"{(int) timestamp.TotalMinutes:00}:{timestamp:ss\\:fff}";
        
        volumeText.Text = $"{point.Volume}%";
        relativeText.Text = $"{point.RelativeScrollSpeed:N2}x";

        if (point.PointType == TimingPointType.Uninherited)
        {
            var uninheritedPoint = (UninheritedTimingPoint) point;
            bpmText.Text = $"{uninheritedPoint.BPM:0.##} BPM";
            signatureText.Text = $"{uninheritedPoint.TimeSignature.Numerator} / {uninheritedPoint.TimeSignature.Denominator}";
        }

        relativeSlider.Current.Disabled = false; // Enable the bindable temporarily to update the slider
        relativeSlider.Current.Value = point.RelativeScrollSpeed;
        relativeSlider.Current.Disabled = true;
    }
}
