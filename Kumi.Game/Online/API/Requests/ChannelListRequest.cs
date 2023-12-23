using Kumi.Game.Online.API.Chat;

namespace Kumi.Game.Online.API.Requests;

public class ChannelListRequest : APIRequest<ChannelListRequest.ChannelListResponse>
{
    public override string Endpoint => "/chat";

    public override HttpMethod Method => HttpMethod.Get;

    protected override APIWebRequest CreateWebRequest() => new ChannelListWebRequest(Uri);

    internal class ChannelListWebRequest : APIWebRequest<ChannelListResponse>
    {
        public ChannelListWebRequest(string? uri)
            : base(uri)
        {
        }
    }

    public class ChannelListResponse : APIResponse
    {
        public APIChatChannel[] GetChannels() => Get<APIChatChannel[]>("channels");
    }
}
