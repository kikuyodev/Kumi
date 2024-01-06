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

    private ChartSetInfoSection infoSection = null!;

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
                    infoSection = new ChartSetInfoSection()
                },
                new Drawable[]
                {
                    new ChartSetListing()
                }
            }
        });

        selectedChartSet.BindValueChanged(v =>
        {
            if (v.NewValue is null)
                infoSection.Hide();
            else
                infoSection.Show();
        });
    }
}
