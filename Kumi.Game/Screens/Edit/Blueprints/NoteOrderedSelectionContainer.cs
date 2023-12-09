using Kumi.Game.Charts.Objects;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Blueprints;

public sealed partial class NoteOrderedSelectionContainer : Container<SelectionBlueprint<Note>>
{
    public override void Add(SelectionBlueprint<Note> drawable)
    {
        SortInternal();
        base.Add(drawable);
    }

    public override bool Remove(SelectionBlueprint<Note> drawable, bool disposeImmediately)
    {
        SortInternal();
        return base.Remove(drawable, disposeImmediately);
    }

    protected override int Compare(Drawable x, Drawable y)
    {
        var xNote = ((SelectionBlueprint<Note>) x).Item;
        var yNote = ((SelectionBlueprint<Note>) y).Item;

        var result = yNote.StartTime.CompareTo(xNote.StartTime);
        if (result != 0)
            return result;
        
        return CompareReverseChildID(x, y);
    }
}
