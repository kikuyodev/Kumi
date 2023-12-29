using Kumi.Game.Graphics.Containers;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Charts;
using Kumi.Game.Online.API.Requests;
using Kumi.Game.Overlays.Listing.Cards;
using Kumi.Game.Overlays.Listing.Filters;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Overlays.Listing;

public partial class ChartSetListing : CompositeDrawable
{
    [Cached]
    private ListingFilters filters = new ListingFilters();

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;
    
    [Resolved]
    private Bindable<APIChartSet?> selectedChartSet { get; set; } = null!;

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
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions = new[]
            {
                new Dimension(),
                new Dimension(GridSizeMode.Absolute, 325)
            },
            RowDimensions = new[]
            {
                new Dimension()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new KumiScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding
                        {
                            Top = 16,
                            Right = 8,
                            Left = 16
                        },
                        Child = cards = new ReverseChildIDFillFlowContainer<ChartSetCard>
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Spacing = new Vector2(6)
                        }
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
        }, 300);
    }
    
    private ChartSetCard[] createCards(APIChartSet[] sets)
        => sets.Select(set => new ChartSetCard(set)
        {
            Action = () => selectedChartSet.Value = set
        }).ToArray();

    protected override bool OnClick(ClickEvent e)
    {
        if (cards.Any(c => c.ReceivePositionalInputAt(e.ScreenSpaceMousePosition)))
            return false;
        
        selectedChartSet.Value = null;
        
        return true;
    }
}
