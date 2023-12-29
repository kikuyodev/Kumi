using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Listing.Filters;

public partial class StatusFilterSelection : ListingLabelledFilter<ChartSetStatus>
{
    protected override Drawable CreateControl() => new FilterEnumSelection<ChartSetStatus>();

    [BackgroundDependencyLoader]
    private void load(ListingFilters filters)
    {
        filters.Status.BindTo(((FilterEnumSelection<ChartSetStatus>)Control).Current);
    }
}
