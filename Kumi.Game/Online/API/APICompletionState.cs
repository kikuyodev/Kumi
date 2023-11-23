namespace Kumi.Game.Online.API;

/// <summary>
/// The state of an API request, ranging from not started to completed.
/// </summary>
public enum APICompletionState
{
    /// <summary>
    /// The request is currently being performed, or is waiting to be performed.
    /// </summary>
    Performing,

    /// <summary>
    /// The request has been completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The request has failed.
    /// </summary>
    Failed
}
