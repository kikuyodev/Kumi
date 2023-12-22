using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class SliderBarSettingItem<T> : LabelledSettingItem<T>
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    protected override Drawable CreateControl() => new SliderBarComponent<T>();
}
