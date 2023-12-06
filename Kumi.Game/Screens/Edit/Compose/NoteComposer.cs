using System.Collections.Specialized;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Components;
using Kumi.Game.Screens.Edit.Compose.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Compose;

public partial class NoteComposer : CompositeDrawable, IPlacementHandler
{
    private ComposeBlueprintContainer blueprintContainer = null!;

    private EditorRadioButtonCollection toolboxCollection = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRangeInternal(new Drawable[]
        {
            blueprintContainer = new ComposeBlueprintContainer
            {
                RelativeSizeAxes = Axes.Both
            },
            toolboxCollection = new EditorRadioButtonCollection
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativePositionAxes = Axes.Y,
                Y = 0.225f
            }
        });

        toolboxCollection.Items = new NoteCompositionTool[]
            {
                new SelectTool(),
                new HitCompositionTool(),
            }.Select(t => new RadioButton(t.Name, () => toolSelected(t), t.CreateIcon))
           .ToList();
        
        editorChart.SelectedNotes.CollectionChanged += selectionChanged;
    }

    private void selectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (editorChart.SelectedNotes.Any())
            setSelectTool();
    }
    
    private void setSelectTool()
        => toolboxCollection.Items.First().Select();
    
    private void toolSelected(NoteCompositionTool tool)
    {
        blueprintContainer.CurrentTool = tool;
        
        if (!(tool is SelectTool))
            editorChart.SelectedNotes.Clear();
    }

    #region IPlacementHandler

    public void BeginPlacement(Note note)
    {
        editorChart.PlacementNote.Value = note;
    }

    public void EndPlacement(Note note, bool commit)
    {
        editorChart.PlacementNote.Value = null;

        if (commit)
            editorChart.Add(note);
    }

    public void Delete(Note note)
    {
        editorChart.Remove(note);
    }

    #endregion
}
