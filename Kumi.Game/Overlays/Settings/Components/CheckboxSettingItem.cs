using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class CheckboxSettingItem : LabelledSettingItem<bool>
{
    protected override Drawable CreateControl() => new CheckboxComponent();
}
