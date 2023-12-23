using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Timeline.Parts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineBlueprintContainer : EditorBlueprintContainer
{
    [Resolved]
    private Timeline? timeline { get; set; }
    
    [Resolved]
    private EditorClock editorClock { get; set; } = null!;
    
    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    private InputManager inputManager = null!;
    
    public TimelineBlueprintContainer()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Height = 0.75f;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = GetContainingInputManager();
    }

    protected override void Update()
    {
        if (IsDragged)
        {
            if (timeline == null)
                return;

            var timelineQuad = timeline.ScreenSpaceDrawQuad;
            var mouseX = inputManager.CurrentState.Mouse.Position.X;
            
            if (mouseX > timelineQuad.TopRight.X)
                timeline.ScrollBy((float)((mouseX - timelineQuad.TopRight.X) / 10 * Clock.ElapsedFrameTime));
            else if (mouseX < timelineQuad.TopLeft.X)
                timeline.ScrollBy((float)((mouseX - timelineQuad.TopLeft.X) / 10 * Clock.ElapsedFrameTime));
        }
        
        base.Update();
    }

    protected override SelectionHandler CreateSelectionHandler()
        => new TimelineSelectionHandler();

    protected override Container<SelectionBlueprint<Note>> CreateSelectionBlueprintContainer()
        => new TimelineSelectionBlueprintContainer();

    protected override SelectionBlueprint<Note>? CreateBlueprintFor(Note note)
        => new TimelineNoteBlueprint(note);

    protected partial class TimelineSelectionBlueprintContainer : Container<SelectionBlueprint<Note>>
    {
        protected override Container<SelectionBlueprint<Note>> Content { get; }

        public TimelineSelectionBlueprintContainer()
        {
            AddInternal(new TimelinePart<SelectionBlueprint<Note>>(Content = new NoteOrderedSelectionContainer
            {
                RelativeSizeAxes = Axes.Both
            })
            {
                RelativeSizeAxes = Axes.Both
            });
        }
    }
}
