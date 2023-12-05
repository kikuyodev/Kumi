using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit;

[Cached]
public partial class ComposeScreen : EditorScreenWithTimeline
{
    private Container playfieldContainer = null!;
    private BlueprintContainer blueprintContainer = null!;

    public KumiPlayfield Playfield { get; private set; } = null!;

    [Resolved]
    private IBindable<WorkingChart> workingChart { get; set; } = null!;

    [Resolved]
    private EditorClock clock { get; set; } = null!;
    
    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    public IReadOnlyList<DrawableNote> Notes => Playfield.Notes;

    public ComposeScreen()
        : base(EditorScreenMode.Compose, false)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

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

            playfieldContainer.Add(new InputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1,
                Children = new Drawable[]
                {
                    Playfield = new KumiPlayfield(workingChart.Value)
                    {
                        Clock = clock,
                        ProcessCustomClock = false
                    },
                    blueprintContainer = new ComposeBlueprintContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Clock = clock,
                        ProcessCustomClock = false
                    }
                }
            });
            
            editorChart.NoteAdded += onNoteAdded;
            editorChart.NoteRemoved += onNoteRemoved;
        });
    }

    private void onNoteAdded(Note note)
    {
        Playfield.Add(note);
    }

    private void onNoteRemoved(Note note)
    {
        Playfield.Remove(note);
    }

    protected override Drawable CreateMainContent()
        => playfieldContainer = new Container { RelativeSizeAxes = Axes.Both };
}
