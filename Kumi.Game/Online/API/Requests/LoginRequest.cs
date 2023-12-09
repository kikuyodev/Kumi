using Kumi.Game.Online.API.Accounts;

namespace Kumi.Game.Online.API.Requests;

public class LoginRequest : APIRequest<LoginRequest.LoginResponse>
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    
    public override string Endpoint => "/accounts/login";

    public override HttpMethod Method => HttpMethod.Post;

    protected override APIWebRequest CreateWebRequest() => new LoginWebRequest(Uri, Username, Password);

    internal class LoginWebRequest : APIWebRequest<LoginResponse>
    {
        public LoginWebRequest(string? uri, string? username, string? password)
            : base(uri)
        {
            AddParameter("username", username);
            AddParameter("password", password);
        }
    }
    
    public class LoginResponse : APIResponse
    {
        public APIAccount GetAccount() => Get<APIAccount>("account");
    }
}
