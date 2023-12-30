namespace Kumi.Game.Database;

/// <summary>
/// A model that can be transmitted to or from the API.
/// </summary>
public interface IModelTransmit<TModel>
    where TModel : class
{
    /// <summary>
    /// Fired when the transmission of the model to or from the API has been started.
    /// </summary>
    event Action? TransmitStarted;
    
    /// <summary>
    /// Fired when the transmission of the model to or from the API has been completed.
    /// </summary>
    event Action<TModel?>? TransmitCompleted;
    
    bool TransmissionInProgress { get; }
    
    /// <summary>
    /// Starts the transmission of the model to or from the API.
    /// </summary>
    void StartTransmit(TModel model);

    /// <summary>
    /// Waits for the transmission to complete.
    /// </summary>
    void WaitForTransmit(TModel model);
}
