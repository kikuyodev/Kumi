using Newtonsoft.Json;

namespace Kumi.Game.Online.API;

public class APIError : IHasStatus
{
    [JsonProperty("code")]
    public int StatusCode { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }
    
    [JsonProperty("errors")]
    public APIValidationError[]? Errors { get; set; }
    
    public class APIValidationError
    {
        [JsonProperty("rule")]
        public string? Rule { get; set; }
        
        [JsonProperty("field")]
        public string? Field { get; set; }
        
        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
