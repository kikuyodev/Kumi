using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Accounts;

public class APIAccount : IUser
{
    /// <summary>
    /// A special ID used to represent the system users, or any other user that is not logged in.
    /// </summary>
    public const int DEFAULT_SYSTEM_ID = 0;
    
    [JsonProperty("id")]
    public int Id { get; set; } = 1;
    
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
}
