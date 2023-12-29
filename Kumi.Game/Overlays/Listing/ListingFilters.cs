using osu.Framework.Bindables;

namespace Kumi.Game.Overlays.Listing;

public class ListingFilters
{
    public Bindable<string> Query { get; set; } = new Bindable<string>();
    
    public Bindable<ChartSetStatus> Status { get; set; } = new Bindable<ChartSetStatus>();
}
