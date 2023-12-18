using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Timeline.Parts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineBlueprintContainer : EditorBlueprintContainer
{
    [Resolved]
    private Timeline? timeline { get; set; }

    public TimelineBlueprintContainer()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Height = 0.75f;
    }

    protected override SelectionHandler CreateSelectionHandler()
        => new TimelineSelectionHandler();

    protected override Container<SelectionBlueprint<Note>> CreateSelectionBlueprintContainer()
        => new TimelineSelectionBlueprintContainer();

    protected override SelectionBlueprint<Note>? CreateBlueprintFor(Note note)
        => new TimelineNoteBlueprint(note)
        {

        };

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
