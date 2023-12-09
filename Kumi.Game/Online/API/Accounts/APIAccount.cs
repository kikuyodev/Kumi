using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Accounts;

public class APIAccount : IAccount
{
    /// <summary>
    /// A special ID used to represent the system users, or any other user that is not logged in.
    /// </summary>
    public const int DEFAULT_SYSTEM_ID = 0;

    [JsonProperty("id")]
    public int Id { get; set; } = 1;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("country")]
    public APICountry Country { get; set; } = new APICountry();
    
    [JsonProperty("groups")]
    public List<APIGroup> Groups { get; set; } = new List<APIGroup>();
    
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
