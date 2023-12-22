using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class DropdownComponent<T> : CompositeDrawable, IHasCurrentValue<T>
{
    public Bindable<T> Current { get; set; } = new Bindable<T>();
    
    private readonly KumiDropdown<T> dropdown;

    public IEnumerable<T> Items
    {
        get => dropdown.Items;
        set => dropdown.Items = value;
    }

    public IBindableList<T> ItemSource
    {
        get => dropdown.ItemSource;
        set => dropdown.ItemSource = value;
    }
    
    public DropdownComponent()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        
        InternalChild = dropdown = CreateDropdown().With(d =>
        {
            d.Width = SettingOverlay.TEXT_BOX_WIDTH;
            d.Anchor = Anchor.TopRight;
            d.Origin = Anchor.TopRight;
        });
    }
    
    protected virtual KumiDropdown<T> CreateDropdown() => new KumiDropdown<T>();

    protected override void LoadComplete()
    {
        base.LoadComplete();
        dropdown.Current.BindTo(Current);
    }
}
