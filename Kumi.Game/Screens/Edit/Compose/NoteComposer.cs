using System.Collections.Specialized;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Components;
using Kumi.Game.Screens.Edit.Compose.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Compose;

public partial class NoteComposer : CompositeDrawable, IPlacementHandler, ISnapProvider
{
    private ComposeBlueprintContainer blueprintContainer = null!;

    private EditorRadioButtonCollection toolboxCollection = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;
    
    [Resolved]
    private ComposeScreen composeScreen { get; set; } = null!;

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
                new DrumCompositionTool(),
                new BigDrumCompositionTool(),
                new DrumRollCompositionTool(),
                new BalloonCompositionTool()
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

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.ControlPressed || e.AltPressed || e.SuperPressed)
            return false;

        if (checkNumberToggleFromKey(e.Key, out var index))
        {
            var item = toolboxCollection.Items.ElementAtOrDefault(index);

            if (item != null)
            {
                if (!item.Selected.Disabled)
                    item.Select();
                
                return true;
            }
        }
        
        return base.OnKeyDown(e);
    }

    private bool checkNumberToggleFromKey(Key key, out int index)
    {
        if (key < Key.Number1 || key > Key.Number9)
        {
            index = -1;
            return false;
        }

        index = key - Key.Number1;
        return true;
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

    #region ISnapProvider

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition)
        => composeScreen.Playfield!.TimeAtScreenSpacePosition(screenSpacePosition);

    public double SnapTime(double time, int beatDivisor)
        => composeScreen.Playfield!.SnapTime(time, beatDivisor);

    #endregion
}
