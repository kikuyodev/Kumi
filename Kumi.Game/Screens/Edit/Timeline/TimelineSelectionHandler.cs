using System.Diagnostics;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineSelectionHandler : SelectionHandler
{
    [Resolved]
    private EditorChart editorChart { get; set; } = null!;
    
    private Note? pivot;
    
    internal override bool MouseDownSelectionRequested(SelectionBlueprint<Note> blueprint, MouseButtonEvent e)
    {
        if (e.ShiftPressed && e.Button == MouseButton.Left && pivot != null)
        {
            handleRangeSelection(blueprint, e.ControlPressed);
            return true;
        }
        
        var result = base.MouseDownSelectionRequested(blueprint, e);
        
        if (editorChart.Notes.Contains(blueprint.Item))
            pivot = blueprint.Item;
        
        return result;
    }

    private void handleRangeSelection(SelectionBlueprint<Note> blueprint, bool union)
    {
        var note = blueprint.Item;

        Debug.Assert(pivot != null);
        
        var rangeStart = Math.Min(note.StartTime, pivot.StartTime);
        var rangeEnd = Math.Max(note.GetEndTime(), pivot.GetEndTime());

        var newSelection = new HashSet<Note>(editorChart.Notes.Where(n => isInRange(n, rangeStart, rangeEnd)).Cast<Note>());

        if (union)
        {
            pivot = note;
            newSelection.UnionWith(editorChart.SelectedNotes);
        }
        
        editorChart.SelectedNotes.Clear();
        editorChart.SelectedNotes.AddRange(newSelection);
        
        bool isInRange(INote n, double start, double end)
            => n.StartTime >= start && n.GetEndTime() <= end;
    }
}
