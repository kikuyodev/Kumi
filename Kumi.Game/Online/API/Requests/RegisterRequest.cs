using Kumi.Game.Online.API.Accounts;

namespace Kumi.Game.Online.API.Requests;

public class RegisterRequest : APIRequest<RegisterRequest.RegisterResponse>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public override string Endpoint => "/accounts/register";
    public override HttpMethod Method => HttpMethod.Post;

    protected override APIWebRequest CreateWebRequest()
        => new RegisterWebRequest(Uri)
        {
            Username = Username,
            Email = Email,
            Password = Password
        };

    internal class RegisterWebRequest : APIWebRequest<RegisterResponse>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;

        public RegisterWebRequest(string? uri)
            : base(uri)
        {
        }

        protected override void PrePerform()
        {
            AddParameter("username", Username);
            AddParameter("email", Email);
            AddParameter("password", Password);
        }
    }

    public class RegisterResponse : APIResponse
    {
        public APIAccount GetAccount() => Get<APIAccount>("account");
    }
}
