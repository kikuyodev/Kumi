using osu.Framework.IO.Network;

namespace Kumi.Game.Online.API.Requests;

public class DownloadChartSetRequest : APIDownloadRequest
{
    public readonly int Id;

    public DownloadChartSetRequest(int id)
    {
        Id = id;
    }

    public override string Endpoint => $"chartsets/{Id}/download";
    public override HttpMethod Method => HttpMethod.Get;

    protected override WebRequest CreateWebRequest()
    {
        var req = base.CreateWebRequest();
        req.Timeout = 60_000;
        return req;
    }
}
