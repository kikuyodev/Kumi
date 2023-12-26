using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Extensions;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Screens.Edit.Timeline;
using Kumi.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose;

[Cached]
public partial class ComposeScreen : EditorScreenWithTimeline, IKeyBindingHandler<PlatformAction>
{
    private NoteComposer noteComposer = null!;
    private Container playfieldContainer = null!;
    private InputManager? inputManager;

    public override bool UpdatePadding => false;

    public KumiPlayfield? Playfield { get; private set; }

    [Resolved]
    private IBindable<WorkingChart> workingChart { get; set; } = null!;

    [Resolved]
    private EditorClock clock { get; set; } = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    [Resolved]
    private EditorHistoryHandler historyHandler { get; set; } = null!;

    public IReadOnlyList<DrawableNote> Notes => Playfield!.Notes;

    public virtual bool CursorInPlacementArea => Playfield?.ReceivePositionalInputAt(inputManager?.CurrentState.Mouse.Position ?? Vector2.Zero) ?? false;

    public ComposeScreen()
        : base(EditorScreenMode.Compose)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
        loadPlayfield();

        editorChart.SelectedNotes.BindCollectionChanged((_, __) => updateClipboardActionAvailability());
        historyHandler.Contents[EditorClipboardType.Note].BindCollectionChanged((_, __) => updateClipboardActionAvailability());
        updateClipboardActionAvailability();
    }

    private void loadPlayfield()
    {
        Schedule(() =>
        {
            if (!workingChart.Value.ChartLoaded || !playfieldContainer.IsLoaded)
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
                noteComposer = new NoteComposer
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

    protected override Drawable CreateTimelineContent()
        => new TimelineBlueprintContainer();

    public override void Copy(bool cut)
    {
        historyHandler.Copy(EditorClipboardType.Note, editorChart.SelectedNotes
           .OrderBy(n => n.StartTime)
           .Select(note => note.ToSaveableString())
           .ToList()
        );

        if (cut)
        {
            editorChart.RemoveRange(editorChart.SelectedNotes);
            editorChart.SelectedNotes.Clear();
        }
    }

    public override void Paste()
    {
        var clipboard = historyHandler.Paste(EditorClipboardType.Note);

        if (clipboard.Count == 0)
            return;

        editorChart.BeginChange();
        editorChart.SelectedNotes.Clear();

        double previousNoteTime = 0.0;
        int pastedNotes = 0;

        foreach (var noteString in clipboard)
        {
            var note = processNote(noteString);

            if (pastedNotes == 0)
            {
                // first note
                previousNoteTime = note.StartTime;
                note.StartTime = clock.CurrentTime;
            }
            else
            {
                note.StartTime = clock.CurrentTime + (note.StartTime - previousNoteTime);
            }

            // find a note at the same time as the pasted note
            var noteAtTime = editorChart.Notes.FirstOrDefault(n => n.StartTime == note.StartTime);

            if (noteAtTime != null)
            {
                editorChart.Remove((Note) noteAtTime);
            }

            editorChart.Add(note);
            editorChart.SelectedNotes.Add(note);
            pastedNotes++;
        }

        editorChart.EndChange();
    }

    private void updateClipboardActionAvailability()
    {
        CanCopy.Value = editorChart.SelectedNotes.Any();
        CanPaste.Value = historyHandler.Contents[EditorClipboardType.Note].Count > 0;
    }

    // TODO: There should be a general way to do this, across both the decoder and the composer
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

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        editorChart.NoteAdded -= onNoteAdded;
        editorChart.NoteRemoved -= onNoteRemoved;
    }

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        switch (e.Action)
        {
            case PlatformAction.SelectAll:
                editorChart.SelectedNotes.Clear();
                editorChart.SelectedNotes.AddRange(editorChart.Notes.Cast<Note>());
                return true;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }
}
