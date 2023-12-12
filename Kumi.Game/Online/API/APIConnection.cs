using System.Net;
using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Online.API.Requests;
using Kumi.Game.Online.API.Requests.Websocket;
using Kumi.Game.Online.Server;
using Kumi.Game.Online.Server.Packets;
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
    public string SessionToken { get; set; }

    public APIConnection(ServerConfiguration configuration)
    {
        EndpointConfiguration = configuration;
        var serverConnector = new ServerConnector(this);
        this.serverConnector = serverConnector;
        
        serverConnector.Closed += b =>
        {
            if (b)
            {
                // The client was intentionally closed.
                return;
            }
            
            // Attempt to reconnect.
            Scheduler.AddDelayed(() =>
            {
                this.serverConnector.AuthorizationToken = string.Empty; // Tokens are invalid after successful connection.
                var tokenReq = new WebsocketTokenRequest();
                
                tokenReq.Success += () =>
                {
                    serverConnector.AuthorizationToken = tokenReq.Response.GetToken();
                    registerConnectorHandlers();
                    
                    // try and connect to the server.
                    serverConnector.Start();
                };
                
                Perform(tokenReq);
            }, 5000L);
        };
    }

    private IServerConnector? serverConnector;
    private CookieContainer cookies = new CookieContainer();
    private bool registeredHandlers;

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
        try
        {
            request.Perform(this);
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to perform {request}");
            request.TriggerFailure(e);
        }
    }

    public void PerformAsync(APIRequest request)
        => Task.Factory.StartNew(() => Perform(request), TaskCreationOptions.LongRunning);

    public void Login(string username, string password)
    {
        var loginReq = new LoginRequest
        {
            Username = username,
            Password = password
        };

        State.Value = APIState.Connecting;

        loginReq.Success += () =>
        {
            try
            {
                LocalAccount.Value = loginReq.Response.GetAccount();
                SessionToken = loginReq.Response.GetToken();
                State.Value = APIState.Online;

                var tokenReq = new WebsocketTokenRequest();
                
                tokenReq.Success += () =>
                {
                    serverConnector.AuthorizationToken = tokenReq.Response.GetToken();
                    registerConnectorHandlers();
                    
                    // try and connect to the server.
                    serverConnector.Start();
                };
                
                Perform(tokenReq);
            }
            catch (KeyNotFoundException e)
            {
                State.Value = APIState.Offline;

                Logger.Error(e, $"Failed to get account from {loginReq}");
                loginReq.TriggerFailure(e);
            }
        };

        try
        {
            loginReq.Perform(this);
        }
        catch (Exception e)
        {
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

    private void registerConnectorHandlers()
    {
        if (registeredHandlers)
            return;
        
        registeredHandlers = true;
        
        this.serverConnector.RegisterPacketHandler<HelloPacket>(OpCode.Hello, performHello);
    }

    private void performHello(HelloPacket packet)
    {
        return;
    }
}
