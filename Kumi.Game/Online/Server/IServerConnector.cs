using Kumi.Game.Online.API;
using osu.Framework.Bindables;

namespace Kumi.Game.Online.Server;

/// <summary>
/// A representation of a connection to the central websocket server.
/// </summary>
public interface IServerConnector
{
    /// <summary>
    /// The current client that is being used.
    /// </summary>
    ServerConnection? CurrentConnection { get; }

    /// <summary>
    /// The provider that created this connection.
    /// </summary>
    IAPIConnectionProvider Provider { get; }

    /// <summary>
    /// Whether or not this connector has been started.
    /// </summary>
    bool Started { get; }

    /// <summary>
    /// Whether or not this connector should automatically start when the API is connected.
    /// </summary>
    bool AutoStart { get; set; }

    /// <summary>
    /// A cancellation token that is used to cancel the connection.
    /// </summary>
    CancellationTokenSource CancellationToken { get; }

    /// <summary>
    /// The current state of the connection.
    /// </summary>
    Bindable<ServerConnectionState> State { get; }

    IDictionary<OpCode, ICollection<Action<Packet>>> PacketHandlers { get; }

    void Start();

    void RegisterPacketHandler<T>(OpCode opCode, Action<T> handler)
        where T : Packet;
}
