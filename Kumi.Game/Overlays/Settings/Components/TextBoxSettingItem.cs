using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class TextBoxSettingItem : LabelledSettingItem<string>
{
    protected override Drawable CreateControl() => new TextBoxComponent();
}
