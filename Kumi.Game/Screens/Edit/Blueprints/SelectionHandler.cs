using Kumi.Game.Charts.Objects;
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

    public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
    {
        switch (e.Action)
        {
            case PlatformAction.Delete:
                DeleteSelected();
                return true;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
    {
    }

    protected void DeleteSelected()
    {
        editorChart.RemoveRange(SelectedItems.ToArray());
        DeselectAll();
    }
}
