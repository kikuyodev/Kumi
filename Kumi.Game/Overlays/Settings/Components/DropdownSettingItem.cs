using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class DropdownSettingItem<T> : LabelledSettingItem<T>
{
    protected override Drawable CreateControl() => new DropdownComponent<T>();
    
    public new DropdownComponent<T> Control => (DropdownComponent<T>)base.Control;
    
    public IEnumerable<T> Items
    {
        get => Control.Items;
        set => Control.Items = value;
    }

    public IBindableList<T> ItemSource
    {
        get => Control.ItemSource;
        set => Control.ItemSource = value;
    }
}
