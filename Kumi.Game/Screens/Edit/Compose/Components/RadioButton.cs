using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit.Compose.Components;

public class RadioButton
{
    public readonly BindableBool Selected;

    public string Label;

    public readonly Func<Drawable>? CreateIcon;

    private readonly Action? action;

    public RadioButton(string label, Action? action, Func<Drawable>? createIcon = null)
    {
        Label = label;
        this.action = action;
        CreateIcon = createIcon;
        
        Selected = new BindableBool();
    }

    public void Select()
    {
        if (!Selected.Value)
        {
            Selected.Value = true;
            action?.Invoke();
        }
    }

    public void Deselect()
        => Selected.Value = false;
}
