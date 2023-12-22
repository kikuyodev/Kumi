using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Compose;
using Kumi.Game.Screens.Edit.Menus;
using Kumi.Game.Screens.Edit.Setup;
using Kumi.Game.Screens.Edit.Timeline;
using Kumi.Game.Screens.Edit.Timing;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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
    
    private MenuItem undoMenuItem = null!;
    private MenuItem redoMenuItem = null!;
    private MenuItem cutMenuItem = null!;
    private MenuItem copyMenuItem = null!;
    private MenuItem pasteMenuItem = null!;

    public IBindable<WorkingChart> Chart { get; } = new Bindable<WorkingChart>();
    
    private readonly IBindable<EditorScreenMode> currentScreen = new Bindable<EditorScreenMode>(EditorScreenMode.Compose);
    
    [Resolved]
    private Editor editor { get; set; } = null!;
    
    [Resolved]
    private EditorHistoryHandler historyHandler { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Chart.BindValueChanged(_ => constructDisplay(), true);
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
                                            undoMenuItem = new MenuItem("Undo", () => editor.Undo()),
                                            redoMenuItem = new MenuItem("Redo", () => editor.Redo()),
                                            cutMenuItem = new MenuItem("Cut", () => editor.Copy(true)),
                                            copyMenuItem = new MenuItem("Copy", () => editor.Copy(false)),
                                            pasteMenuItem = new MenuItem("Paste", () => editor.Paste()),
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
                                Colour = Colours.GRAY_C,
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
                                    Colour = Colours.Gray(0.05f)
                                },
                            },
                            new EditorScreenTabControl
                            {
                                // Current = { Value = EditorScreenMode.Compose }
                                Current = { BindTarget = currentScreen }
                            }
                        }
                    }
                }
            },
            new BottomBarTimeline
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            }
        };
        
        historyHandler.CanUndo.BindValueChanged(v => undoMenuItem.Action.Disabled = !v.NewValue, true);
        historyHandler.CanRedo.BindValueChanged(v => redoMenuItem.Action.Disabled = !v.NewValue, true);
        
        currentScreen.BindValueChanged(v =>
        {
            EditorScreen newScreen;

            switch (v.NewValue)
            {
                default:
                case EditorScreenMode.Compose:
                    newScreen = new ComposeScreen();
                    break;
                case EditorScreenMode.Setup:
                    newScreen = new SetupScreen();
                    break;
                case EditorScreenMode.Timing:
                    newScreen = new TimingScreen();
                    break;
            }

            editor.PushScreen(newScreen);
        });
        
        editor.CurrentScreen.BindValueChanged(v =>
        {
            // unbind from previous screen
            if (v.OldValue != null)
            {
                v.OldValue.CanCopy.UnbindAll();
                v.OldValue.CanPaste.UnbindAll();
            }
            
            // bind to new screen
            if (v.NewValue != null)
            {
                v.NewValue.CanCopy.BindValueChanged(c =>
                {
                    copyMenuItem.Action.Disabled = !c.NewValue;
                    cutMenuItem.Action.Disabled = !c.NewValue;
                }, true);
                v.NewValue.CanPaste.BindValueChanged(c => pasteMenuItem.Action.Disabled = !c.NewValue, true);
            }
        }, true);
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
