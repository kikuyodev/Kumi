using Kumi.Game.Overlays.Listing.Filters;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays.Listing;

public partial class ChartListingSection : CompositeDrawable
{
    [Cached]
    private ListingFilters filters = new ListingFilters();

    public ChartListingSection()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            ColumnDimensions = new[]
            {
                new Dimension(),
                new Dimension(GridSizeMode.Absolute, 325)
            },
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new[]
                {
                    Empty(),
                    new FiltersSection()
                }
            }
        };
    }
}
