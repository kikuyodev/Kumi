using Kumi.Game.Online.API.Charts;
using osu.Framework.Extensions;

namespace Kumi.Game.Online.API.Requests;

public class UploadChartSetRequest : APIRequest<UploadChartSetRequest.UploadChartSetResponse>
{
    public Stream ChartSetStream { get; set; } = null!;
    public bool IsWip { get; set; }
    
    public override string Endpoint => "/chartsets/submit";
    public override HttpMethod Method => HttpMethod.Post;

    protected override APIWebRequest CreateWebRequest() => new UploadChartSetWebRequest(Uri, ChartSetStream, IsWip);

    internal class UploadChartSetWebRequest : APIWebRequest<UploadChartSetResponse>
    {
        public UploadChartSetWebRequest(string? uri, Stream uploadStream, bool isWip)
            : base(uri)
        {
            AddFile("set", uploadStream.ReadAllBytesToArray());
            AddParameter("status", (isWip ? 0 : 1).ToString());
        }
    }
    
    public class UploadChartSetResponse : APIResponse
    {
        public UploadedChartResponse[] GetUploadedCharts() => GetMeta<UploadedChartResponse[]>("charts");
        public APIChartSet GetUploadedSet() => Get<APIChartSet>("set");
    }
}