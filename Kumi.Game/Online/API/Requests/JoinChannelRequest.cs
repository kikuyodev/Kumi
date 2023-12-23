using Kumi.Game.Online.API.Chat;

namespace Kumi.Game.Online.API.Requests;

public class JoinChannelRequest : APIRequest
{
    public APIChatChannel Channel { get; init; } = null!;

    public override string Endpoint => $"/chat/{Channel.Id}/join";

    public override HttpMethod Method => HttpMethod.Put;

    protected override APIWebRequest CreateWebRequest() => new JoinChannelWebRequest(Uri);
    
    internal class JoinChannelWebRequest : APIWebRequest
    {
        public JoinChannelWebRequest(string? uri)
            : base(uri)
        {
        }
    }
}
