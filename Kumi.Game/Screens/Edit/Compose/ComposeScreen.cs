using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Screens.Edit.Compose;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osuTK;

namespace Kumi.Game.Screens.Edit;

[Cached]
public partial class ComposeScreen : EditorScreenWithTimeline
{
    private Container playfieldContainer = null!;
    private InputManager? inputManager;

    public KumiPlayfield? Playfield { get; private set; }

    [Resolved]
    private IBindable<WorkingChart> workingChart { get; set; } = null!;

    [Resolved]
    private EditorClock clock { get; set; } = null!;
    
    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    public IReadOnlyList<DrawableNote> Notes => Playfield!.Notes;

    public virtual bool CursorInPlacementArea => Playfield?.ReceivePositionalInputAt(inputManager?.CurrentState.Mouse.Position ?? Vector2.Zero) ?? false;

    public ComposeScreen()
        : base(EditorScreenMode.Compose, false)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
        loadPlayfield();
    }

    private void loadPlayfield()
    {
        Schedule(() =>
        {
            if (!workingChart.Value.ChartLoaded)
            {
                loadPlayfield();
                return;
            }
            
            playfieldContainer.AddRange(new Drawable[]
            {
                new InputBlockingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1,
                    Child = Playfield = new KumiPlayfield(workingChart.Value)
                    {
                        Clock = clock,
                        ProcessCustomClock = false
                    }
                },
                new NoteComposer
                {
                    RelativeSizeAxes = Axes.Both,
                }
            });
            
            editorChart.NoteAdded += onNoteAdded;
            editorChart.NoteRemoved += onNoteRemoved;
        });
    }

    private void onNoteAdded(Note note)
    {
        Playfield?.Add(note);
    }

    private void onNoteRemoved(Note note)
    {
        Playfield?.Remove(note);
    }

    protected override Drawable CreateMainContent()
        => playfieldContainer = new Container { RelativeSizeAxes = Axes.Both };

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        
        editorChart.NoteAdded -= onNoteAdded;
        editorChart.NoteRemoved -= onNoteRemoved;
    }
}
