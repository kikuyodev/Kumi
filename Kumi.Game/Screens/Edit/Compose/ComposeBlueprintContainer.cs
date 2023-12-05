using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit.Compose;

public partial class ComposeBlueprintContainer : EditorBlueprintContainer
{
    protected override SelectionBlueprint<Note>? CreateBlueprintFor(Note note)
    {
        var drawable = Composer?.Notes.FirstOrDefault(d => d.Note == note);

        if (drawable == null)
            return null;

        return CreateNoteBlueprintFor(note)?.With(b => b.DrawableNote = drawable);
    }

    public virtual NoteSelectionBlueprint? CreateNoteBlueprintFor(Note note)
        => note switch
        {
            DrumHit drumHit => drumHit.Flags.Value.HasFlagFast(NoteFlags.Big)
                ? new BigDrumNoteSelectionBlueprint(note)
                : new DrumNoteSelectionBlueprint(note),
            _ => null
        };
}
