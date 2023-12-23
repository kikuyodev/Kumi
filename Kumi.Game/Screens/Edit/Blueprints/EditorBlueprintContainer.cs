using System.Collections.Specialized;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Compose;
using osu.Framework.Allocation;

namespace Kumi.Game.Screens.Edit.Blueprints;

public partial class EditorBlueprintContainer : BlueprintContainer
{
    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    [Resolved]
    private EditorChart chart { get; set; } = null!;

    [Resolved]
    protected ComposeScreen? Composer { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        SelectedItems.BindTo(chart.SelectedNotes);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        chart.NoteAdded += AddBlueprintFor;
        chart.NoteRemoved += RemoveBlueprintFor;
        chart.SelectedNotes.CollectionChanged += updateSelectionLifetime;

        if (Composer != null)
        {
            Scheduler.AddDelayed(() =>
            {
                foreach (var note in Composer.Notes)
                    AddBlueprintFor((Note) note.Note);
            }, 10);
        }
    }

    protected override void AddBlueprintFor(Note note)
    {
        if (note is BarLine)
            return;
        
        base.AddBlueprintFor(note);
    }

    private void updateSelectionLifetime(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.NewItems != null)
        {
            foreach (Note item in args.NewItems)
                Composer?.Playfield?.SetKeepAlive(item, true);
        }

        if (args.OldItems != null)
        {
            foreach (Note item in args.OldItems)
                Composer?.Playfield?.SetKeepAlive(item, false);
        }
    }

    protected override void OnBlueprintSelected(SelectionBlueprint<Note> blueprint)
    {
        base.OnBlueprintSelected(blueprint);
        Composer?.Playfield?.SetKeepAlive(blueprint.Item, true);
    }

    protected override void OnBlueprintDeselected(SelectionBlueprint<Note> blueprint)
    {
        base.OnBlueprintDeselected(blueprint);
        Composer?.Playfield?.SetKeepAlive(blueprint.Item, false);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        chart.NoteAdded -= AddBlueprintFor;
        chart.NoteRemoved -= RemoveBlueprintFor;
        chart.SelectedNotes.CollectionChanged -= updateSelectionLifetime;
    }
}
