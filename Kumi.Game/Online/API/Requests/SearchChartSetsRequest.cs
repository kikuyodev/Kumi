using Kumi.Game.Online.API.Charts;
using osu.Framework.IO.Network;

namespace Kumi.Game.Online.API.Requests;

public class SearchChartSetsRequest : APIRequest<SearchChartSetsRequest.SearchChartSetsResponse>
{
    public string? Query { get; set; }

    public override string Endpoint => "/chartsets/search";
    public override HttpMethod Method => HttpMethod.Get;

    protected override APIWebRequest CreateWebRequest() => new SearchChartSetsWebRequest(Uri, new Dictionary<string, string>
    {
        { "query", Query ?? string.Empty }
    });

    internal class SearchChartSetsWebRequest : APIWebRequest<SearchChartSetsResponse>
    {
        public SearchChartSetsWebRequest(string? uri, Dictionary<string, string> parameters)
            : base(uri)
        {
            foreach (var (key, value) in parameters)
                AddParameter(key, value, RequestParameterType.Query);
        }
    }

    public class SearchChartSetsResponse : APIResponse
    {
        public APIChartSet[] GetChartSets() => Get<APIChartSet[]>("results");
    }
}
