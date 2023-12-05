using osu.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace Kumi.Game.Screens.Edit.Blueprints;

public abstract partial class SelectionBlueprint<T> : CompositeDrawable, IStateful<SelectionState>
{
    public readonly T Item;

    public event Action<SelectionBlueprint<T>>? Selected;
    public event Action<SelectionBlueprint<T>>? Deselected;

    public override bool HandlePositionalInput => IsSelectable;
    public override bool RemoveWhenNotAlive => false;

    protected SelectionBlueprint(T item)
    {
        Item = item;

        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateState();
    }

    private SelectionState state;
    
    public event Action<SelectionState>? StateChanged;

    public SelectionState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;
            
            if (IsLoaded)
                updateState();
            
            StateChanged?.Invoke(state);
        }
    }

    private void updateState()
    {
        switch (state)
        {
            case SelectionState.Selected:
                OnSelected();
                Selected?.Invoke(this);
                break;
            case SelectionState.NotSelected:
                OnDeselected();
                Deselected?.Invoke(this);
                break;
        }
    }

    protected virtual void OnDeselected()
    {
        foreach (var d in InternalChildren)
            d.Hide();
    }

    protected virtual void OnSelected()
    {
        foreach (var d in InternalChildren)
            d.Show();
    }

    protected override bool ShouldBeConsideredForInput(Drawable child) => State == SelectionState.Selected;

    public void Select() => State = SelectionState.Selected;
    
    public void Deselect() => State = SelectionState.NotSelected;

    public void ToggleSelection() => State = IsSelected ? SelectionState.NotSelected : SelectionState.Selected;
    
    public bool IsSelected => State == SelectionState.Selected;
    
    public virtual bool IsSelectable => ShouldBeAlive && IsPresent;

    public virtual Vector2 ScreenSpaceSelectionPoint => ScreenSpaceDrawQuad.Centre;

    public virtual Quad SelectionQuad => ScreenSpaceDrawQuad;
}

public enum SelectionState
{
    NotSelected,
    Selected,
}
