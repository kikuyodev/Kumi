using Kumi.Game.Online.API;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace Kumi.Game.Online.Server;

public class ServerConnector : IServerConnector//, IDisposable
{
    public ServerConnection CurrentConnection { get; private set; }
    public IAPIConnectionProvider Provider { get; }
    public bool Started { get; private set; }
    public bool AutoStart { get; set; } = true;
    public Bindable<ServerConnectionState> State { get; } = new Bindable<ServerConnectionState>(ServerConnectionState.Disconnected);

    public Dictionary<ServerPacketOpCode, List<Action<ServerPacket>>> PacketHandlers { get; } = new Dictionary<ServerPacketOpCode, List<Action<ServerPacket>>>();
    public CancellationTokenSource CancellationToken { get; private set; } = new CancellationTokenSource();
    
    public ServerConnector(IAPIConnectionProvider provider, bool autoStart = true)
    {
        Provider = provider;
        apiState.BindTo(provider.State);
        AutoStart = autoStart;

        if (AutoStart)
        {
            Start();
        }
    }
    
    private readonly IBindable<APIState> apiState = new Bindable<APIState>();
    private readonly SemaphoreSlim connectionLock = new SemaphoreSlim(1);
    private IDictionary<ServerPacketOpCode, ICollection<Action<ServerPacket>>> packetHandlers = new Dictionary<ServerPacketOpCode, ICollection<Action<ServerPacket>>>();

    public void Start()
    {
        if (Started)
            return;
        
        apiState.BindValueChanged(c =>
        {
            switch (c.NewValue)
            {
                case APIState.Failed:
                case APIState.Offline:
                    disconnect();
                    break;
                
                case APIState.Online:
                    connect();
                    break;
            }
        });
        
        Started = true;
    }
    public void RegisterPacketHandler<T>(ServerPacketOpCode opCode, Action<T> handler)
        where T : ServerPacket
    {
        var castingHandler = (ServerPacket packet) =>
        {
            try
            {
                // Take the raw data and deserialize it into the specified type.
                T data = JsonConvert.DeserializeObject<T>(packet.RawData);
                data.RawData = packet.RawData;
                
                handler(data);
            } catch (JsonException)
            {
                // Likely a malformed packet, or wasn't meant for this handler.
            } catch (Exception e)
            {
                Logger.Error(e, $"Failed to handle packet {opCode}.");
            }
        };
        
        // Register the packet handler.
        if (!PacketHandlers.TryGetValue(opCode, out var handlers))
        {
            handlers = new List<Action<ServerPacket>>();
            PacketHandlers.Add(opCode, handlers);
        }
        
        // Add the handler to the list.
        handlers.Add(castingHandler);
    }

    private async Task connect()
    {
        if (CurrentConnection != null && CurrentConnection.IsConnected)
        {
            cancelExistingConnection();
        }
        
        if (!await connectionLock.WaitAsync(1000))
            throw new TimeoutException("Failed to acquire connection lock, likely due to a deadlock.");

        try
        {
            var cancellationToken = CancellationToken.Token;
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                State.Value = ServerConnectionState.Connecting;
                CurrentConnection = new ServerConnection(this);
                await CurrentConnection.ConnectAsync(cancellationToken).ConfigureAwait(false);
                
                CurrentConnection.Closed += e =>
                {
                    if (e != null)
                        Logger.Log($"Connection closed: {e.Message}", LoggingTarget.Network);
                    
                    if (cancellationToken.IsCancellationRequested)
                        return true;
                    
                    State.Value = ServerConnectionState.Disconnected;
                    return true;
                };
                
                CurrentConnection.PacketReceived += packet =>
                {
                    if (PacketHandlers.TryGetValue(packet.OpCode, out var handlers))
                    {
                        foreach (var handler in handlers)
                            handler(packet);
                    }
                };

                State.Value = ServerConnectionState.Connected;
                cancellationToken.ThrowIfCancellationRequested();
            } catch (OperationCanceledException)
            {
                // The connection was cancelled.
            } catch (Exception e)
            {
                // The connection failed.
                Logger.Log($"Failed to connect to the server: {e.Message}", LoggingTarget.Network);
                State.Value = ServerConnectionState.Failed;
            }
        } catch (OperationCanceledException)
        {
            // The connection was cancelled.
        } catch (Exception e)
        {
            // The connection failed.
            Logger.Log($"Failed to connect to the server: {e.Message}", LoggingTarget.Network);
            State.Value = ServerConnectionState.Failed;
        } finally
        {
            connectionLock.Release();
        }
    }
    
    private async Task disconnect()
    {
        cancelExistingConnection();

        try
        {
            if (CurrentConnection != null)
            {
                await CurrentConnection.DisposeAsync();
            }
        } finally {
            CurrentConnection = null;
        }
    }
    
    private void cancelExistingConnection()
    {
        CancellationToken.Cancel();
        CancellationToken = new CancellationTokenSource();
    }

    IDictionary<ServerPacketOpCode, ICollection<Action<ServerPacket>>> IServerConnector.PacketHandlers => packetHandlers;
}
