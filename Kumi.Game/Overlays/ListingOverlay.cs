using Kumi.Game.Audio;
using Kumi.Game.Online.API.Charts;
using Kumi.Game.Overlays.Listing;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays;

public partial class ListingOverlay : OnlineOverlay
{
    [Resolved]
    private TrackPreviewManager previewManager { get; set; } = null!;
    
    [Cached]
    private Bindable<APIChartSet?> selectedChartSet { get; set; } = new Bindable<APIChartSet?>();

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions = new[]
            {
                new Dimension()
            },
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize),
                new Dimension()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new ChartSetInfoSection()
                },
                new Drawable[]
                {
                    new ChartSetListing()
                }
            }
        });
    }
}
