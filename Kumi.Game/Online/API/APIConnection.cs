using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Online.Server;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace Kumi.Game.Online.API;

public partial class APIConnection : Component, IAPIConnectionProvider
{

    public ServerConfiguration EndpointConfiguration { get; }
    public Queue<APIRequest> RequestQueue { get; private set; } = new Queue<APIRequest>();
    public Bindable<APIAccount> LocalAccount { get; } = new Bindable<APIAccount>(new GuestAccount());
    public Bindable<APIState> State { get; } = new Bindable<APIState>(APIState.Offline);
    public string SessionToken { get; }

    public APIConnection(ServerConfiguration configuration)
    {
        EndpointConfiguration = configuration;
        serverConnector = new ServerConnector(this);
    }
    
    private IServerConnector? serverConnector;
    
    public new void Schedule(Action action) => base.Schedule(action);
    
    public void Queue(APIRequest request) => RequestQueue.Enqueue(request);
    
    public void ForceDequeue(APIRequest request)
    {
        // reconstruct the queue to place the request at the front.
        var queue = new Queue<APIRequest>();
        queue.Enqueue(request);
        
        while (RequestQueue.Count > 0)
            queue.Enqueue(RequestQueue.Dequeue());
        
        RequestQueue = queue;
    }

    public void Perform(APIRequest request)
    {
        try {
            request.Perform(this);
        } catch (Exception e) {
            Logger.Error(e, $"Failed to perform {request}");
            request.TriggerFailure(e);
        }
    }

    public void PerformAsync(APIRequest request)
        => Task.Factory.StartNew(() => Perform(request), TaskCreationOptions.LongRunning);
    
    public void Login(string username, string password)
    {
        throw new NotImplementedException();
    }
    public void Logout()
    {
        throw new NotImplementedException();
    }

    public IServerConnector? GetServerConnector() => serverConnector;

    #region IAPIConnectionProvider implementation
    
    IBindable<APIAccount> IAPIConnectionProvider.LocalAccount => LocalAccount;
    IBindable<APIState> IAPIConnectionProvider.State => State;

    #endregion
    
}
