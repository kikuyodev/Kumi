using Kumi.Game.Audio;
using Kumi.Game.Overlays.Listing;
using osu.Framework.Allocation;

namespace Kumi.Game.Overlays;

public partial class ListingOverlay : OnlineOverlay
{
    [Resolved]
    private TrackPreviewManager previewManager { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new ChartListingSection());
    }
}
