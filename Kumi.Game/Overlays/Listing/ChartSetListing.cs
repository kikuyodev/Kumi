using Kumi.Game.Graphics.Containers;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Charts;
using Kumi.Game.Online.API.Requests;
using Kumi.Game.Overlays.Listing.Cards;
using Kumi.Game.Overlays.Listing.Filters;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Overlays.Listing;

public partial class ChartSetListing : CompositeDrawable
{
    [Cached]
    private ListingFilters filters = new ListingFilters();

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;

    public ChartSetListing()
    {
        RelativeSizeAxes = Axes.Both;
    }

    private ReverseChildIDFillFlowContainer<ChartSetCard> cards = null!;
    
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
                new Drawable[]
                {
                    cards = new ReverseChildIDFillFlowContainer<ChartSetCard>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(6),
                    },
                    new FiltersSection()
                }
            }
        };

        filters.Query.ValueChanged += _ => debounceRequest();
        api.State.BindValueChanged(v =>
        {
            if (v.NewValue == APIState.Online)
                debounceRequest();
        }, true);
    }

    private ScheduledDelegate? filterChangeDebounce;

    private void debounceRequest()
    {
        filterChangeDebounce?.Cancel();
        filterChangeDebounce = null;

        filterChangeDebounce = Scheduler.AddDelayed(() =>
        {
            filterChangeDebounce = null;

            if (api.State.Value == APIState.Offline)
                return;

            var req = new SearchChartSetsRequest { Query = filters.Query.Value };

            req.Success += () =>
            {
                cards.ChildrenEnumerable = createCards(req.Response.GetChartSets());
            };

            api.PerformAsync(req);
        }, 100);
    }
    
    private ChartSetCard[] createCards(APIChartSet[] sets)
        => sets.Select(set => new ChartSetCard(set)).ToArray();
}
