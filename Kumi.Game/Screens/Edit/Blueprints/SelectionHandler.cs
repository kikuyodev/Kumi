using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Extensions;
using Kumi.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Blueprints;

public partial class SelectionHandler : CompositeDrawable, IKeyBindingHandler<PlatformAction>
{
    [Resolved]
    private EditorChart editorChart { get; set; } = null!;
    
    [Resolved]
    private EditorHistoryHandler historyHandler { get; set; } = null!;
    
    [Resolved]
    private EditorClock editorClock { get; set; } = null!;
    
    public IReadOnlyList<SelectionBlueprint<Note>> SelectedBlueprints => selectedBlueprints;
    
    public readonly BindableList<Note> SelectedItems = new BindableList<Note>();
    
    private readonly List<SelectionBlueprint<Note>> selectedBlueprints;

    public SelectionHandler()
    {
        selectedBlueprints = new List<SelectionBlueprint<Note>>();

        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
    }

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;
    

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        switch (e.Action)
        {
            case PlatformAction.Delete:
                DeleteSelected();
                return true;
            
            case PlatformAction.Cut:
                CopySelected(true);
                return true;
            
            case PlatformAction.Copy:
                CopySelected();
                return true;
            
            case PlatformAction.Paste:
                Paste();
                return true;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }
    
    public void CopySelected(bool cut = false)
    {
        if (SelectedItems.Count == 0)
            return;

        var clipboard = new List<Note>(SelectedItems);

        historyHandler.Copy(EditorClipboardType.Note, serializeNotes(clipboard.OrderBy(n => n.StartTime)));

        if (cut)
        {
            editorChart.RemoveRange(clipboard);
            DeselectAll();
        }
    }
    
    public void Paste()
    {
        var clipboard = historyHandler.Paste(EditorClipboardType.Note);
        
        if (clipboard == null || clipboard.Count == 0)
            return;
        
        editorChart.BeginChange();

        double previousNoteTime = 0.0;
        int pastedNotes = 0;
        
        foreach (var noteString in clipboard)
        {
            var note = processNote(noteString);

            if (pastedNotes == 0)
            {
                // first note
                previousNoteTime = note.StartTime;
                note.StartTime = editorClock.CurrentTime;
            }
            else
            {
                note.StartTime = editorClock.CurrentTime + (note.StartTime - previousNoteTime);
                previousNoteTime = note.StartTime;
            }
            
            // find a note at the same time as the pasted note
            var noteAtTime = editorChart.Notes.FirstOrDefault(n => n.StartTime == note.StartTime);
            
            if (noteAtTime != null)
            {
                editorChart.Remove((Note)noteAtTime);
            }
            
            editorChart.Add(note);
            pastedNotes++;
        }
        
        editorChart.EndChange();
    }

    #region Deletion
    
    protected void DeleteSelected()
    {
        editorChart.RemoveRange(SelectedItems.ToArray());
        DeselectAll();
    }

    #endregion

    #region Selection

    protected void DeselectAll()
        => SelectedItems.Clear();

    internal void HandleSelected(SelectionBlueprint<Note> blueprint)
    {
        if (!SelectedItems.Contains(blueprint.Item))
            SelectedItems.Add(blueprint.Item);

        selectedBlueprints.Add(blueprint);
    }

    internal void HandleDeselected(SelectionBlueprint<Note> blueprint)
    {
        SelectedItems.Remove(blueprint.Item);
        selectedBlueprints.Remove(blueprint);
    }

    internal bool MouseDownSelectionRequested(SelectionBlueprint<Note> blueprint, MouseButtonEvent e)
    {
        if (e is { ShiftPressed: true, Button: MouseButton.Left } && !blueprint.IsSelected)
        {
            blueprint.ToggleSelection();
            return true;
        }
        
        if (blueprint.IsSelected)
            return false;
        
        DeselectAll();
        blueprint.Select();
        return true;
    }

    internal bool MouseUpSelectionRequested(SelectionBlueprint<Note> blueprint, MouseButtonEvent e)
    {
        if (blueprint.IsSelected)
        {
            blueprint.ToggleSelection();
            return true;
        }

        return false;
    }

    #endregion
    
    private List<string> serializeNotes(IEnumerable<Note> notes)
    {
        var noteStrings = new List<string>();
        
        foreach (var note in notes)
        {
            noteStrings.Add(note.ToSaveableString());
        }

        return noteStrings;
    }
    
    private Note processNote(string line)
    {
        var args = line.SplitComplex(Note.DELIMITER).ToArray();
        var typeValue = (NoteType) StringUtils.AssertAndFetch<int>(args[0]);
        Note? note;

        switch (typeValue)
        {
            case NoteType.Don:
            case NoteType.Kat:
                note = new DrumHit(StringUtils.AssertAndFetch<float>(args[1]));
                note.Flags.Value = (NoteFlags) StringUtils.AssertAndFetch<int>(args[2]);
                break;

            case NoteType.Drumroll:
                var drumroll = new DrumRoll(StringUtils.AssertAndFetch<float>(args[1]));
                drumroll.EndTime = StringUtils.AssertAndFetch<float>(args[2]);
                drumroll.Flags.Value = (NoteFlags) StringUtils.AssertAndFetch<int>(args[3]);

                note = drumroll;
                break;

            case NoteType.Balloon:
                var balloon = new Balloon(StringUtils.AssertAndFetch<float>(args[1]));
                balloon.EndTime = StringUtils.AssertAndFetch<float>(args[2]);
                balloon.Flags.Value = (NoteFlags) StringUtils.AssertAndFetch<int>(args[3]);

                note = balloon;
                break;

            default:
                throw new InvalidDataException($"Invalid note type: {typeValue}");
        }

        // TODO: Temporary
        note.Windows = new NoteWindows();
        note.Type.Value = typeValue;

        return note;
    }
}
