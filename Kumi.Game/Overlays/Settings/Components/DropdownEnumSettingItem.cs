namespace Kumi.Game.Overlays.Settings.Components;

public partial class DropdownEnumSettingItem<T> : DropdownSettingItem<T>
    where T : struct, Enum
{
    public DropdownEnumSettingItem()
    {
        Items = Enum.GetValues<T>();
    }
}
