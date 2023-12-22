using Kumi.Game.Online.API.Accounts;
using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Chat;

public class APIChatMessage
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("content")]
    public string Content { get; set; }
    
    [JsonProperty("channel")]
    public APIChatChannel Channel { get; set; }
    
    [JsonProperty("account")]
    public APIAccount Account { get; set; } 
}