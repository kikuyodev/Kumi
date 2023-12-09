using System.Net;
using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Online.API.Requests;
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
    private CookieContainer cookies = new CookieContainer();

    public new void Schedule(Action action) => Scheduler.Add(action);
    
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
        Console.WriteLine($"Logging in as {username} with password {password}");
        var loginReq = new LoginRequest()
        {
            Username = username,
            Password = password
        };

        loginReq.Success += () =>
        {
            State.Value = APIState.Online;
            LocalAccount.Value = loginReq.Response.GetAccount();
        };
        
        try
        {
            loginReq.Perform(this);
        } catch (Exception e) {
            Logger.Error(e, $"Failed to perform {loginReq}");
            loginReq.TriggerFailure(e);
        }
    }
    public void Logout()
    {
        throw new NotImplementedException();
    }

    public IServerConnector? GetServerConnector() => serverConnector;

    #region IAPIConnectionProvider implementation
    
    CookieContainer IAPIConnectionProvider.Cookies => cookies;
    IBindable<APIAccount> IAPIConnectionProvider.LocalAccount => LocalAccount;
    IBindable<APIState> IAPIConnectionProvider.State => State;

    #endregion
    
}
