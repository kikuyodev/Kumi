using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Blueprints;

public partial class SelectionHandler<T> : CompositeDrawable
{
    public IReadOnlyList<SelectionBlueprint<T>> SelectedBlueprints => selectedBlueprints;
    
    public readonly BindableList<T> SelectedItems = new BindableList<T>();
    
    private readonly List<SelectionBlueprint<T>> selectedBlueprints;

    public SelectionHandler()
    {
        selectedBlueprints = new List<SelectionBlueprint<T>>();

        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
    }

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

    protected void DeselectAll()
        => SelectedItems.Clear();

    internal void HandleSelected(SelectionBlueprint<T> blueprint)
    {
        if (!SelectedItems.Contains(blueprint.Item))
            SelectedItems.Add(blueprint.Item);

        selectedBlueprints.Add(blueprint);
    }

    internal void HandleDeselected(SelectionBlueprint<T> blueprint)
    {
        SelectedItems.Remove(blueprint.Item);
        selectedBlueprints.Remove(blueprint);
    }

    internal bool MouseDownSelectionRequested(SelectionBlueprint<T> blueprint, MouseButtonEvent e)
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

    internal bool MouseUpSelectionRequested(SelectionBlueprint<T> blueprint, MouseButtonEvent e)
    {
        if (blueprint.IsSelected)
        {
            blueprint.ToggleSelection();
            return true;
        }

        return false;
    }
}
