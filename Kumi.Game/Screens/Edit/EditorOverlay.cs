using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Menus;
using Kumi.Game.Screens.Edit.Timeline;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Screens.Edit;

[Cached]
public partial class EditorOverlay : Container
{
    public const float TOP_BAR_HEIGHT = 24;

    public IBindable<WorkingChart> Chart { get; } = new Bindable<WorkingChart>();

    public EditorOverlay()
    {
        Chart.BindValueChanged(_ => constructDisplay());
    }

    private void constructDisplay()
    {
        Padding = new MarginPadding(12);

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = TOP_BAR_HEIGHT,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(12, 0),
                        Children = new Drawable[]
                        {
                            new EditorMenuBar(Direction.Horizontal)
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.Y,
                                Items = new[]
                                {
                                    new MenuItem("File")
                                    {
                                        Items = new[]
                                        {
                                            new MenuItem("Exit")
                                        }
                                    },
                                    new MenuItem("Edit")
                                    {
                                        Items = new[]
                                        {
                                            new MenuItem("Undo"),
                                            new MenuItem("Redo"),
                                            new MenuItem("Cut"),
                                            new MenuItem("Copy"),
                                            new MenuItem("Paste"),
                                            new MenuItem("Clone"),
                                        }
                                    },
                                    new MenuItem("View")
                                    {
                                        Items = new[]
                                        {
                                            new MenuItem("Zoom In"),
                                            new MenuItem("Zoom Out"),
                                            new MenuItem("Zoom to Selection"),
                                        }
                                    },
                                    new MenuItem("Settings")
                                }
                            },
                            new SpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Alpha = 0.5f,
                                Colour = Color4Extensions.FromHex("CCCCCC"),
                                Text = getRomanisableString(),
                                Font = KumiFonts.GetFont(size: 12)
                            }
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Width = 200,
                        RelativeSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Masking = true,
                                CornerRadius = 5,
                                RelativeSizeAxes = Axes.Both,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4Extensions.FromHex("0D0D0D")
                                },
                            },
                            new EditorScreenTabControl
                            {
                                Current = { Value = EditorScreenMode.Compose }
                            }
                        }
                    }
                }
            },
            new TopBarTimeline
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            }
        };
    }
    
    private RomanisableString getRomanisableString()
    {
        if (Chart.Value == null)
            return new RomanisableString(null, null);
        
        var original = $"{Chart.Value.Metadata.Artist} - {Chart.Value.Metadata.Title} [{Chart.Value.Chart.ChartInfo.DifficultyName}]";
        var romanised = $"{Chart.Value.Metadata.ArtistRomanised} - {Chart.Value.Metadata.TitleRomanised} [{Chart.Value.Chart.ChartInfo.DifficultyName}]";

        return new RomanisableString(original, romanised);
    }
}
