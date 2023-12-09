using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Accounts;

public class APICountry
{
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("native")]
    public string NativeName { get; set; } = string.Empty;
    
}
