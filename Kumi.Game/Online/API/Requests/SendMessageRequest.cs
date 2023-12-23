using Kumi.Game.Online.API.Chat;

namespace Kumi.Game.Online.API.Requests;

public class SendMessageRequest : APIRequest
{
    public string Content { get; init; } = string.Empty;
    public APIChatChannel Channel { get; init; } = null!;

    public override string Endpoint => $"/chat/{Channel.Id}/messages";

    public override HttpMethod Method => HttpMethod.Post;

    protected override APIWebRequest CreateWebRequest() => new SendMessageWebRequest(Uri, Content);

    internal class SendMessageWebRequest : APIWebRequest
    {
        public SendMessageWebRequest(string? uri, string content)
            : base(uri)
        {
            AddParameter("content", content);
        }
    }
}
