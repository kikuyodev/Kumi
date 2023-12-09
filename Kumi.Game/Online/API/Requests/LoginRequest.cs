using Kumi.Game.Online.API.Accounts;

namespace Kumi.Game.Online.API.Requests;

public class LoginRequest : APIRequest<LoginRequest.LoginResponse>
{
    public string Username { get; set; }
    public string Password { get; set; }
    
    public override string Endpoint { get; } = "/accounts/login";

    public override HttpMethod Method { get; } = HttpMethod.Post;

    protected override APIWebRequest CreateWebRequest() => new LoginWebRequest(Uri, Username, Password);

    internal class LoginWebRequest : APIWebRequest<LoginResponse>
    {
        public LoginWebRequest(string? uri, string username, string passsword)
            : base(uri)
        {
            this.AddParameter("username", username);
            this.AddParameter("password", passsword);
        }
    }
    
    public class LoginResponse : APIResponse
    {
        public APIAccount GetAccount() => Get<APIAccount>("account");
    }
}
