using Kumi.Game.Input;
using Kumi.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Kumi.Game.Graphics.Containers;

public abstract partial class KumiFocusedOverlayContainer : FocusedOverlayContainer, IKeyBindingHandler<GlobalAction>
{
    protected override bool BlockNonPositionalInput => true;
    
    protected virtual bool DimMainContent => true;
    
    [Resolved]
    private IOverlayManager? overlayManager { get; set; }

    public virtual bool BlockScreenWideMouse => BlockPositionalInput;

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => BlockScreenWideMouse || base.ReceivePositionalInputAt(screenSpacePos);

    private bool closeOnMouseUp;

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        closeOnMouseUp = !base.ReceivePositionalInputAt(e.ScreenSpaceMousePosition);
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (closeOnMouseUp && !base.ReceivePositionalInputAt(e.ScreenSpaceMousePosition))
            Hide();
        
        base.OnMouseUp(e);
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case GlobalAction.Back:
                Hide();
                return true;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    protected override void UpdateState(ValueChangedEvent<Visibility> state)
    {
        switch (state.NewValue)
        {
            case Visibility.Visible:
                if (BlockScreenWideMouse && DimMainContent)
                    overlayManager?.ShowBlockingOverlay(this);
                break;
            case Visibility.Hidden:
                if (BlockScreenWideMouse)
                    overlayManager?.HideBlockingOverlay(this);
                break;
        }
        
        base.UpdateState(state);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        overlayManager?.HideBlockingOverlay(this);
    }
}
