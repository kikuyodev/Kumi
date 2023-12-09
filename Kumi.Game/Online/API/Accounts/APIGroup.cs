using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Accounts;

public class APIGroup
{
    [JsonProperty("id")]
    public int Id { get; set; } = 1;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("tag")]
    public string Tag { get; set; } = string.Empty;
    
    [JsonProperty("identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [JsonProperty("color")]
    public string Color { get; set; } = string.Empty;
    
    [JsonProperty("priority")]
    public int Priority { get; set; } = 255;
    
    [JsonProperty("visible")]
    public bool Visible { get; set; }
    
    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonProperty("description")]
    public string? Description { get; set; } = string.Empty;
    
    [JsonProperty("has_page")]
    public bool HasPage { get; set; }
}