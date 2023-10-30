using Newtonsoft.Json;
using osu.Framework.IO.Network;

namespace Kumi.Game.Online.API;

/// <summary>
/// A web request to the API with a parsed response.
/// </summary>
public class APIWebRequest<T> : APIWebRequest
    where T : IHasStatus
{
    public APIWebRequest(string? uri)
        : base(uri)
    {}

    protected override void ProcessResponse()
    {
        string response = GetResponseString();

        if (!typeof(T).IsClass)
        {
            base.ProcessResponse();
            return;
        }
        
        if (!string.IsNullOrEmpty(response))
        {
            ResponseObject = JsonConvert.DeserializeObject<T>(response);
        }
        
        base.ProcessResponse();
    }
    
    public T? ResponseObject { get; private set; }
}

/// <summary>
/// A web request to the API without a parsed response.
/// </summary>
public class APIWebRequest : WebRequest
{
    public event APIRequestFailed? Failure;
    
    public event APIRequestSucceeded? Success;
    
    public APIWebRequest(string? uri)
        : base(uri)
    {
        this.Failed += onFailure;
    }

    protected override void ProcessResponse()
    {
        base.ProcessResponse();
        
        Success?.Invoke();
    }

    protected override string UserAgent => "Kumi";

    public APIError? LastError { get; private set; }

    private void onFailure(Exception e)
    {
        if (!(e is OperationCanceledException))
        {
            // Get the response string, if any.
            string? response = GetResponseString();

            // If the response string is not null or empty, try to deserialize it.
            if (!string.IsNullOrEmpty(response))
            {
                LastError = JsonConvert.DeserializeObject<APIError>(response);
            }
        }

        // Execute the failure event.
        Failure?.Invoke(new APIRequestException(LastError, e.Message));
    }

    public delegate void APIRequestFailed(Exception e);
    public delegate void APIRequestSucceeded();
}
