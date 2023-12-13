using Kumi.Game.Online.API;
using Kumi.Game.Online.Server.Packets;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Logging;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace Kumi.Game.Online.Server;

public class ServerConnector : IServerConnector //, IDisposable
{
    public ServerConnection? CurrentConnection { get; private set; }
    public IAPIConnectionProvider Provider { get; }
    public bool Started { get; private set; }
    public bool AutoStart { get; set; }
    public string AuthorizationToken { get; set; } = string.Empty;
    public Bindable<ServerConnectionState> State { get; } = new Bindable<ServerConnectionState>(ServerConnectionState.Disconnected);
    public CancellationTokenSource CancellationToken { get; private set; } = new CancellationTokenSource();

    public event Action<bool> Closed;

    public ServerConnector(IAPIConnectionProvider provider, bool autoStart = true)
    {
        Provider = provider;
        apiState.BindTo(provider.State);
        AutoStart = autoStart;

        if (AutoStart)
            Start();
        
        RegisterPacketHandler<DispatchPacket<object>>(OpCode.Dispatch, packet =>
        {
            // Forward the packet to the relevant dispatch handlers.
            if (dispatchHandlers.TryGetValue(packet.DispatchType, out var handlers))
            {
                foreach (var handler in handlers)
                    handler(packet);
            }
        });
    }

    private readonly IBindable<APIState> apiState = new Bindable<APIState>();
    private readonly SemaphoreSlim connectionLock = new SemaphoreSlim(1);
    private IDictionary<OpCode, List<Action<Packet>>> packetHandlers { get; } = new Dictionary<OpCode, List<Action<Packet>>>();
    private IDictionary<string, List<Action<Packet>>> dispatchHandlers { get; } = new Dictionary<string, List<Action<Packet>>>();

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

    public void RegisterPacketHandler<T>(OpCode opCode, Action<T> handler)
        where T : Packet
    {
        void castingHandler(Packet packet)
        {
            try
            {
                // Take the raw data and deserialize it into the specified type.
                var data = JsonConvert.DeserializeObject<T>(packet.RawData);
                data!.RawData = packet.RawData;

                handler(data);
            }
            catch (JsonException)
            {
                // Likely a malformed packet, or wasn't meant for this handler.
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to handle packet {opCode}.");
            }
        }

        // Register the packet handler.
        if (!packetHandlers.TryGetValue(opCode, out var handlers))
        {
            handlers = new List<Action<Packet>>();
            packetHandlers.Add(opCode, handlers);
        }

        // Add the handler to the list.
        handlers.Add(castingHandler);
    }
    
    public void RegisterDispatchHandler<T>(string dispatch, Action<T> handler)
        where T : Packet
    {
        void castingHandler(Packet packet)
        {
            try
            {
                // Take the raw data and deserialize it into the specified type.
                var data = JsonConvert.DeserializeObject<T>(packet.RawData);
                data!.RawData = packet.RawData;
                
                if (data.OpCode != OpCode.Dispatch)
                    return;
                
                if (data.DispatchType != dispatch)
                    return;

                handler(data);
            }
            catch (JsonException)
            {
                // Likely a malformed packet, or wasn't meant for this handler.
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to handle packet {dispatch}.");
            }
        }

        // Register the packet handler.
        if (!dispatchHandlers.TryGetValue(dispatch, out var handlers))
        {
            handlers = new List<Action<Packet>>();
            dispatchHandlers.Add(dispatch, handlers);
        }

        // Add the handler to the list.
        handlers.Add(castingHandler);
    }

    private async Task connect()
    {
        cancelExistingConnection();

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


                CurrentConnection.PacketReceived += packet =>
                {
                    if (packetHandlers.TryGetValue(packet.OpCode, out var handlers))
                        foreach (var handler in handlers)
                            handler(packet);
                };

                State.Value = ServerConnectionState.Connected;
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                // The connection was cancelled.
                Logger.Log("Connection cancelled.", LoggingTarget.Network);
                State.Value = ServerConnectionState.Disconnected;
                Closed?.Invoke(true);
            }
            catch (Exception e)
            {
                // The connection failed.
                Logger.Log($"Failed to connect to the server: {e.Message}", LoggingTarget.Network);
                Closed?.Invoke(false);
                State.Value = ServerConnectionState.Failed;
            }
        }
        catch (OperationCanceledException)
        {
            // The connection was cancelled.
        }
        catch (Exception e)
        {
            // The connection failed.
            Logger.Log($"Failed to connect to the server: {e.Message}", LoggingTarget.Network);
            State.Value = ServerConnectionState.Failed;
        }
        finally
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
                await CurrentConnection.DisposeAsync();
        }
        finally
        {
            CurrentConnection = null;
        }
    }

    private void cancelExistingConnection()
    {
        CancellationToken.Cancel();
        CancellationToken = new CancellationTokenSource();
    }

    IDictionary<OpCode, List<Action<Packet>>> IServerConnector.PacketHandlers => packetHandlers;

    IDictionary<string, List<Action<Packet>>> IServerConnector.DispatchHandlers => dispatchHandlers;

}
