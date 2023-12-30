using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class SimpleStatisticRow : ChartStatisticRow
{
    public new LocalisableString Label
    {
        get => base.Label;
        set => base.Label = value;
    }
    
    public new LocalisableString Value
    {
        get => base.Value;
        set => base.Value = value;
    }
    
    protected override Drawable CreateVisualisation() => Empty();
}
