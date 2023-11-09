using Newtonsoft.Json;

namespace Kumi.Game.Online.API;

/// <summary>
/// An interface for API responses that have a status code and a possible error message.
/// </summary>
public interface IHasStatus
{
    /// <summary>
    /// The status code of the response.
    /// </summary>
    [JsonProperty("code")]
    public int StatusCode { get; set; }

    /// <summary>
    /// A possible error message, just in case the status code is not 200.
    /// </summary>
    [JsonProperty("error")]
    public string? ErrorMessage { get; set; }
}
