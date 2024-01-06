using Kumi.Game.Database;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Requests;
using osu.Framework.Allocation;
using Realms;

namespace Kumi.Game.Charts;

public partial class ChartDownloadTransmit : ModelTransmit<ChartSetInfo, DownloadChartSetRequest>
{
    public event Action<DownloadChartSetRequest>? ModifyRequest;

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    public ChartDownloadTransmit(RealmAccess realm, IAPIConnectionProvider api)
        : base(realm, api)
    {
    }

    protected override DownloadChartSetRequest CreateRequest(ChartSetInfo model)
    {
        var request = new DownloadChartSetRequest(model.OnlineID!.Value);
        ModifyRequest?.Invoke(request);

        return request;
    }

    protected override ChartSetInfo? ProcessResponse(ChartSetInfo model, DownloadChartSetRequest request, Realm realm)
    {
        var set = realm.Find<ChartSetInfo>(model.ID);

        if (set is null)
        {
            chartManager.Import(new[] { new ImportTask(request.Filename) }).ConfigureAwait(false);
        }
        else
        {
            // import as update
        }

        return model;
    }
}
