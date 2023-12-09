using System.Net;
using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Online.Server;
using osu.Framework.Bindables;

namespace Kumi.Game.Online.API;

/// <summary>
/// An object that represents a connection to the API.
/// </summary>
public interface IAPIConnectionProvider
{
    /// <summary>
    /// The current endpoint configuration that this provider is using.
    /// </summary>
    ServerConfiguration EndpointConfiguration { get; }

    /// <summary>
    /// The current queue of requests that are waiting to be executed.
    /// </summary>
    Queue<APIRequest> RequestQueue { get; }

    /// <summary>
    /// The current account that is logged in.
    /// </summary>
    IBindable<APIAccount> LocalAccount { get; }

    /// <summary>
    /// The current session token that is being used.
    /// </summary>
    string SessionToken { get; }

    /// <summary>
    /// The current state of the API connection.
    /// </summary>
    IBindable<APIState> State { get; }
    
    /// <summary>
    /// Cookies that are used for requests.
    /// </summary>
    CookieContainer Cookies { get; }

    /// <summary>
    /// Passes a function to be executed on the game's update thread.
    /// </summary>
    /// <param name="action">The function to execute.</param>
    public void Schedule(Action action);

    /// <summary>
    /// Queues a request to be executed.
    /// </summary>
    /// <param name="request">The request to execute.</param>
    public void Queue(APIRequest request);

    /// <summary>
    /// Forces a request to be dequeued and executed.
    /// This should only be used for requests that need to be executed semi-immediately.
    /// </summary>
    /// <param name="request">The request to forcefully run.</param>
    public void ForceDequeue(APIRequest request);

    /// <summary>
    /// Performs a request, usually immediately.
    /// </summary>
    /// <param name="request">The request to perform.</param>
    public void Perform(APIRequest request);

    /// <summary>
    /// Asynchronously performs a request.
    /// </summary>
    /// <param name="request">The request to perform.</param>
    public void PerformAsync(APIRequest request);

    /// <summary>
    /// Attempts to login to the API with the provided credentials.
    /// </summary>
    /// <param name="username">The username of the account.</param>
    /// <param name="password">The password to the account.</param>
    public void Login(string username, string password);

    /// <summary>
    /// Logs the current account out of the API, invalidating the session token in the process.
    /// </summary>
    public void Logout();

    /// <summary>
    /// Creates a new server connector for the web socket. May return null if the provider does not support web sockets, or if
    /// the provider is not connected.
    /// </summary>
    IServerConnector? GetServerConnector();
}
