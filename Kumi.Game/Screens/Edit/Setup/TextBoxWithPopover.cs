using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace Kumi.Game.Screens.Edit.Setup;

public abstract partial class TextBoxWithPopover : CompositeDrawable, IHasPopover
{
    public abstract Popover? GetPopover();

    protected TextBoxWithPopover()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        
        InternalChild = new PopoverTextBox
        {
            RelativeSizeAxes = Axes.X,
            Height = 30,
            OnFocused = this.ShowPopover
        };
    }
    
    internal partial class PopoverTextBox : KumiTextBox
    {
        public Action? OnFocused;

        protected override bool OnDragStart(DragStartEvent e)
            => false;

        protected override void OnFocus(FocusEvent e)
        {
            if (Current.Disabled)
                return;
            
            OnFocused?.Invoke();
            base.OnFocus(e);
            
            GetContainingInputManager().TriggerFocusContention(this);
        }
    }
}
