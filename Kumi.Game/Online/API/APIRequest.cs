namespace Kumi.Game.Online.API;

public abstract class APIRequest<T>
    : APIRequest
    where T : APIResponse
{
    public T Response { get; private set; } = null!;

    protected APIRequest()
    {
        Success += () =>
        {
            var saferRequest = (APIWebRequest<T>) Request;
            Response = saferRequest.ResponseObject!;
        };
    }
}

/// <summary>
/// An object that represents a request to the API, without any response data.
/// </summary>
public abstract class APIRequest
{
    /// <summary>
    /// The endpoint that this request will be sent to.
    /// </summary>
    public abstract string Endpoint { get; }
    
    public virtual HttpMethod Method { get; } = HttpMethod.Get;

    /// <summary>
    /// The query parameters that this request will be sent with.
    /// </summary>
    public Dictionary<string, string> QueryParameters { get; } = new Dictionary<string, string>();

    /// <summary>
    /// The current state of this request.
    /// </summary>
    public APICompletionState CompletionState { get; private set; } = APICompletionState.Performing;

    /// <summary>
    /// An event that is triggered when this request has succeeded.
    /// When possible, this event will be triggered on the update thread.
    /// </summary>
    public event APIWebRequest.APIRequestSucceeded? Success;

    /// <summary>
    /// An event that is triggered when this request has failed.
    /// When possible, this event will be triggered on the update thread.
    /// </summary>
    public event APIWebRequest.APIRequestFailed? Failure;

    protected virtual string Uri => Path.Join(Provider!.EndpointConfiguration.APIUri, Endpoint);

    protected virtual APIWebRequest CreateWebRequest() => new APIWebRequest($@"{Uri}?{string.Join("&", QueryParameters.Select(x => $"{x.Key}={x.Value}"))}");

    protected IAPIConnectionProvider? Provider;

    protected APIWebRequest Request = null!;

    private readonly object completionStateMutex = new object();

    /// <summary>
    /// Performs this request.
    /// </summary>
    public void Perform(IAPIConnectionProvider? provider)
    {
        Provider = provider;
        Request = CreateWebRequest();

        // Assign specific headers here.
        Request.Method = Method;
        Request.AddHeader("Cookie", provider?.Cookies.ToString());
        
        // TODO: Think of headers to assign.
        Request.Failure += TriggerFailure;
        Request.Success += TriggerSuccess;

        if (isFailing)
            return;

        try
        {
            Request.Perform();
        }
        catch
        {
            // ignored
        }

        if (isFailing)
            return;

        TriggerSuccess();
    }

    public void Cancel() => TriggerFailure(new OperationCanceledException("The request was manually cancelled."));

    internal void TriggerSuccess()
    {
        lock (completionStateMutex)
        {
            if (CompletionState != APICompletionState.Performing)
                return;

            CompletionState = APICompletionState.Completed;
        }

        if (Provider != null)
            Provider.Schedule(() => Success?.Invoke());
        else
            Success?.Invoke();
    }

    internal void TriggerFailure(Exception e)
    {
        lock (completionStateMutex)
        {
            if (CompletionState != APICompletionState.Performing)
                return;

            CompletionState = APICompletionState.Failed;
        }

        if (Provider != null)
            Provider.Schedule(() => Failure?.Invoke(e));
        else
            Failure?.Invoke(e);
    }

    private bool isFailing
    {
        get
        {
            lock (completionStateMutex)
            {
                return CompletionState == APICompletionState.Failed;
            }
        }
    }
}
