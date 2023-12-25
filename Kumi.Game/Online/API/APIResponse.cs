using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kumi.Game.Online.API;

/// <summary>
/// An object that represents a response from the API, without any response data.
/// </summary>
public class APIResponse : IHasStatus
{
    [JsonProperty("code")]
    public int StatusCode { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }
    
    [JsonProperty("data")]
    public JObject? Data { get; set; }
    
    [JsonProperty("meta")]
    public JObject? Meta { get; set; }
    
    [JsonIgnore]
    public bool IsSuccess => StatusCode == 200;
    
    [JsonIgnore]
    public CookieContainer Cookies { get; set; } = new CookieContainer();
   
    /// <summary>
    /// Gets the value of the specified key from the response data.
    /// </summary>
    /// <param name="key">The key to get the value of.</param>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <exception cref="KeyNotFoundException">An exception that states the key was not found.</exception>
    public T Get<T>(string key)
    {
        if (Data is null)
            throw new KeyNotFoundException($"Key {key} was not found in the response data.");
        
        if (Data.TryGetValue(key, out var value))
        {
            if (value.Type == JTokenType.Object)
                return JsonConvert.DeserializeObject<T>(value.ToString())!;

            // Cast it to the type we want.
            return (T) value.ToObject(typeof(T))!;
        }

        throw new KeyNotFoundException($"Key {key} was not found in the response data.");
    }
   
    /// <summary>
    /// Gets the value of the specified key from the response meta.
    /// </summary>
    /// <param name="key">The key to get the value of.</param>
    /// <typeparam name="T">The type to return.</typeparam>
    /// <exception cref="KeyNotFoundException">An exception that states the key was not found.</exception>
    public T GetMeta<T>(string key)
    {
        if (Meta is null)
            throw new KeyNotFoundException($"Key {key} was not found in the response meta.");
        
        if (Meta.TryGetValue(key, out var value))
        {
            if (value.Type == JTokenType.Object)
                return JsonConvert.DeserializeObject<T>(value.ToString())!;

            // Cast it to the type we want.
            return (T) value.ToObject(typeof(T))!;
        }

        throw new KeyNotFoundException($"Key {key} was not found in the response meta.");
    }
}
