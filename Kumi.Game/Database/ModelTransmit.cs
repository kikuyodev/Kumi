using Kumi.Game.Online.API;
using osu.Framework.Graphics;
using osu.Framework.Threading;
using Realms;

namespace Kumi.Game.Database;

public abstract partial class ModelTransmit<TModel, TRequest> : Component, IModelTransmit<TModel>
    where TModel : RealmObjectBase
    where TRequest : APIRequest
{
    public event Action? TransmitStarted;
    public event Action<TModel?>? TransmitCompleted;

    public bool TransmissionInProgress => transmitTask != null;

    private readonly RealmAccess realm;
    private readonly IAPIConnectionProvider api;

    protected ModelTransmit(RealmAccess realm, IAPIConnectionProvider api)
    {
        this.realm = realm;
        this.api = api;
    }

    private Task? transmitTask;

    public void StartTransmit(TModel model)
    {
        if (TransmissionInProgress)
            throw new InvalidOperationException("Transmission is already in progress.");

        if (api.State.Value == APIState.Offline)
            throw new InvalidOperationException("Cannot transmit while offline.");

        TransmitStarted?.Invoke();
        transmitTask = Task.Factory.StartNew(() => transmit(model), TaskCreationOptions.LongRunning);
    }

    private ScheduledDelegate? waitForTransmitDelegate;

    public void WaitForTransmit(TModel model)
    {
        Scheduler.Add(waitForTransmitDelegate = new ScheduledDelegate(() =>
        {
            if (transmitTask is not { IsCompleted: true })
                return;

            transmitTask = null;
            waitForTransmitDelegate?.Cancel();
            waitForTransmitDelegate = null;

            finishTransmit(model);
        }, 0, 10));
    }

    private TRequest? request;

    private void transmit(TModel model)
    {
        request = CreateRequest(model);
        api.Perform(request);
        
        OnRequestFinished(request);
    }

    protected virtual void OnRequestFinished(TRequest req)
    {
    }

    private void finishTransmit(TModel model)
        => realm.Write(r =>
        {
            var newModel = ProcessResponse(model, request!, r);

            TransmitCompleted?.Invoke(newModel);
        });

    protected abstract TRequest CreateRequest(TModel model);

    protected abstract TModel? ProcessResponse(TModel model, TRequest response, Realm realm);
}
