using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Kumi.Game.Overlays.Listing.Filters;

public partial class QueryFilter : ListingLabelledFilter<string>
{
    protected override Drawable CreateControl() => new KumiTextBox
    {
        RelativeSizeAxes = Axes.X,
        Height = 24,
        PlaceholderText = "Search title, artist, creator...",
        FontSize = 14
    };

    protected override bool ShowLabel => false;

    [BackgroundDependencyLoader]
    private void load(ListingFilters filters)
    {
        filters.Query.BindTo(((KumiTextBox)Control).Current);
    }
}
