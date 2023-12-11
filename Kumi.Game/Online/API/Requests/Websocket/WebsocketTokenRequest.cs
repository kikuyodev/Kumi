namespace Kumi.Game.Online.API.Requests.Websocket;

public class WebsocketTokenRequest : APIRequest<WebsocketTokenRequest.WebsocketTokenResponse>
{
    public override string Endpoint => "/websocket/token";

    public override HttpMethod Method => HttpMethod.Get;

    protected override APIWebRequest CreateWebRequest() => new WebsocketTokenWebRequest(Uri);

    internal class WebsocketTokenWebRequest : APIWebRequest<WebsocketTokenResponse>
    {
        public WebsocketTokenWebRequest(string? uri)
            : base(uri)
        {
        }
    }
    
    public class WebsocketTokenResponse : APIResponse
    {
        public string? GetToken() => Get<string>("token");
    }
}
