using Kumi.Game.Database;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Requests;
using osu.Framework.Allocation;
using Realms;

namespace Kumi.Game.Charts;

public partial class ChartUploadTransmit : ModelTransmit<ChartSetInfo, UploadChartSetRequest>
{
    public event Action<UploadChartSetRequest>? ModifyRequest;

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    public ChartUploadTransmit(RealmAccess realm, IAPIConnectionProvider api)
        : base(realm, api)
    {
    }
    
    private Stream? chartSetStream;

    protected override UploadChartSetRequest CreateRequest(ChartSetInfo model)
    {
        chartSetStream?.Dispose();
        chartSetStream = new MemoryStream();
        chartManager.ExportModelToStream(model, chartSetStream);

        chartSetStream.Seek(0, SeekOrigin.Begin);

        var request = new UploadChartSetRequest { ChartSetStream = chartSetStream };
        ModifyRequest?.Invoke(request);

        return request;
    }

    protected override void OnRequestFinished(UploadChartSetRequest req)
    {
        chartSetStream?.Dispose();
    }

    protected override ChartSetInfo? ProcessResponse(ChartSetInfo model, UploadChartSetRequest request, Realm realm)
    {
        var response = request.Response;
        
        if (!response.IsSuccess)
            return null;
        
        var uploadedCharts = response.GetUploadedCharts();
        var uploadedSet = response.GetUploadedSet();

        var set = realm.Find<ChartSetInfo>(model.ID);
        if (set is null)
            return null;
        
        set.OnlineID = uploadedSet.Id;
        foreach (var chartInfo in set.Charts)
        {
            var uploadedChart = uploadedCharts.FirstOrDefault(c => c.OriginalHash == chartInfo.Hash);
            if (uploadedChart is null)
                continue;

            chartInfo.OnlineID = uploadedChart.Chart.Id;
        }
        
        return set.Detach();
    }
}
