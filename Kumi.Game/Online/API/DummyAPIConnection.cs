using Kumi.Game.Online.API.Users;
using Kumi.Game.Online.Server;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Game.Online.API;

public partial class DummyAPIConnection : Component, IAPIConnectionProvider
{
    public ServerConfiguration EndpointConfiguration { get; } = new DummyServerConfiguration();

    public Queue<APIRequest> RequestQueue { get; private set; } = new Queue<APIRequest>();

    // For testing.
    internal bool PauseQueue = false;

    public readonly Bindable<APIAccount> LocalAccount = new Bindable<APIAccount>(new GuestAccount());
    
    public readonly Bindable<APIState> State = new Bindable<APIState>();

    public string SessionToken { get; } = "dummy";

    /// <summary>
    /// Passes a function to be executed on the game's update thread, whenever a request is made and ready to be executed.
    /// </summary>
    public Func<APIRequest, bool>? HandleRequest { get; set; }

    public new void Schedule(Action action) => base.Schedule(action);

    public DummyAPIConnection()
    {
        serverConnector = new ServerConnector(this, false);
    }

    private IServerConnector? serverConnector;

    public void Queue(APIRequest request)
    {
        RequestQueue.Enqueue(request);
    }

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
        => perform(request);

    public void PerformAsync(APIRequest request)
        => Task.Factory.StartNew(() => perform(request));
    
    public void Login(string username, string password)
    {
        State.Value = APIState.Connecting;

        // Create a new account object.
        LocalAccount.Value = new APIAccount
        {
            Id = 1,
            Username = "Dummy",
        };

        State.Value = APIState.Online;
    }
    public void Logout()
    {
        State.Value = APIState.Offline;
        LocalAccount.Value = new GuestAccount();
    }

    public IServerConnector? GetServerConnector() => serverConnector;

    protected override void Update()
    {
        base.Update();

        if (RequestQueue.Count > 0 && !PauseQueue)
            while (RequestQueue.TryDequeue(out var request))
                Perform(request);
        
        // Update the state of the connector
    }

    private void perform(APIRequest request)
    {
        //If the request is not handled, throw an exception.
        if (HandleRequest == null)
        {
            request.TriggerFailure(new InvalidOperationException($@"The {nameof(DummyAPIConnection)} cannot execute requests currently."));
        }
        
        if (HandleRequest?.Invoke(request) == false)
            request.TriggerFailure(new InvalidOperationException($@"The {nameof(DummyAPIConnection)} cannot execute requests currently."));
        else
            request.TriggerSuccess();
    }
    
    IBindable<APIAccount> IAPIConnectionProvider.LocalAccount => LocalAccount;
    IBindable<APIState> IAPIConnectionProvider.State => State;

    internal class DummyServerConfiguration : ServerConfiguration
    {
        public DummyServerConfiguration()
        {
            WebsocketUri = "wss://echo.websocket.events/";
        }
    }
}
