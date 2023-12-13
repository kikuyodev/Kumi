using Kumi.Game.Charts;
using Kumi.Game.Charts.Drawables;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Control;

public partial class MusicControlSection : Container
{
    private const int height = 100;

    private SpriteText titleText = null!;
    private SpriteText artistText = null!;
    private MusicProgressBar progressBar = null!;
    private KumiIconButton playButton = null!;

    private ChartBackgroundSprite? currentBackground;
    private Container backgroundContainer = null!;

    private WorkingChart? currentChart;

    [Resolved]
    private IBindable<WorkingChart> workingChart { get; set; } = null!;

    [Resolved]
    private MusicController controller { get; set; } = null!;

    public MusicControlSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(8);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            Height = height,
            Masking = true,
            CornerRadius = 5f,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        backgroundContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientHorizontal(Colours.Gray(0.05f).Opacity(0.9f), Colours.Gray(0.05f).Opacity(0.5f))
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(2),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding(4),
                            Children = new Drawable[]
                            {
                                titleText = new SpriteText
                                {
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                    Colour = Colours.Gray(0.9f),
                                    Text = "Title"
                                },
                                artistText = new SpriteText
                                {
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat),
                                    Colour = Colours.Gray(0.7f),
                                    Text = "Artist"
                                }
                            }
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Margin = new MarginPadding { Bottom = 10 },
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(4, 0),
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Padding = new MarginPadding(4),
                            Children = new Drawable[]
                            {
                                new KumiIconButton
                                {
                                    Size = new Vector2(20),
                                    IconScale = new Vector2(0.6f),
                                    Icon = FontAwesome.Solid.AngleLeft,
                                    Action = () => controller.Previous(),
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Colour = Colours.GRAY_C
                                },
                                playButton = new KumiIconButton
                                {
                                    Size = new Vector2(20),
                                    IconScale = new Vector2(0.6f),
                                    Icon = FontAwesome.Solid.PlayCircle,
                                    Action = () => controller.TogglePause(),
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Colour = Colours.GRAY_C
                                },
                                new KumiIconButton
                                {
                                    Size = new Vector2(20),
                                    IconScale = new Vector2(0.6f),
                                    Icon = FontAwesome.Solid.AngleRight,
                                    Action = () => controller.Next(),
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Colour = Colours.GRAY_C
                                },
                            }
                        },
                        progressBar = new MusicProgressBar(true)
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 6,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            OnSeek = controller.SeekTo
                        }
                    }
                }
            }
        };

        workingChart.BindValueChanged(_ => updateContents(), true);
    }

    protected override void Update()
    {
        base.Update();

        var track = controller.CurrentTrack;

        if (!track.IsDummyDevice)
        {
            progressBar.EndTime = track.Length;
            progressBar.CurrentTime = track.CurrentTime;
            playButton.Icon = track.IsRunning ? FontAwesome.Solid.PauseCircle : FontAwesome.Solid.PlayCircle;
        }
        else
        {
            progressBar.CurrentTime = 0;
            progressBar.EndTime = 1;
            playButton.Icon = FontAwesome.Solid.PlayCircle;
        }
    }

    private void updateContents()
    {
        if (workingChart.Value == currentChart)
            return;

        if (workingChart.Value is DummyWorkingChart)
            return;

        currentChart = workingChart.Value;

        titleText.Text = new RomanisableString(currentChart.Metadata.Title, currentChart.Metadata.TitleRomanised);
        artistText.Text = new RomanisableString(currentChart.Metadata.Artist, currentChart.Metadata.ArtistRomanised);
        pushBackground();
    }

    private int backgroundDepth;

    private void pushBackground()
    {
        if (currentBackground == null && currentChart != null)
        {
            // Initial background
            currentBackground = createBackground();
            backgroundContainer.Add(currentBackground);
            return;
        }

        var newBackground = createBackground();
        backgroundContainer.Add(newBackground);

        currentBackground?.FadeOut(200).OnComplete(_ =>
        {
            backgroundContainer.Remove(currentBackground, true);
            currentBackground = newBackground;
        });
    }

    private ChartBackgroundSprite createBackground()
        => new ChartBackgroundSprite(currentChart!)
        {
            Depth = backgroundDepth--,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fill,
        };
}
