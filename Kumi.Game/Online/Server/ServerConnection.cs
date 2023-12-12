using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Kumi.Game.Online.Server.Packets;
using Newtonsoft.Json;
using osu.Framework.Logging;
using SixLabors.ImageSharp;

namespace Kumi.Game.Online.Server;

/// <summary>
/// An abstraction of a connection to a server.
/// </summary>
public class ServerConnection : IAsyncDisposable
{
    /// <summary>
    /// The current connection to the server.
    /// </summary>
    public ClientWebSocket Connection { get; }

    /// <summary>
    /// The connector that created this connection.
    /// </summary>
    public IServerConnector Connector { get; }

    /// <summary>
    /// The thread that is used to update the connection, and receive messages.
    /// </summary>
    public Thread? ConnectionThread { get; private set; }

    public bool IsConnected => Connection.State == WebSocketState.Open;

    public event Func<Exception?, bool>? Closed;

    public event Action<byte[]>? RawMessageReceived;

    public event Action<Packet>? PacketReceived;

    public ServerConnection(IServerConnector connector)
    {
        Connector = connector;
        Connection = new ClientWebSocket();
        
        PacketReceived += packet =>
        {
            if (packet.OpCode != OpCode.Pong)
                return;
            
            expectingPong = false;
            onPong?.Invoke();
        };
    }

    private readonly Queue<byte[]> messageQueue = new Queue<byte[]>();

    private readonly CancellationToken cancellationToken = default;
    private bool expectingPong;
    private Func<bool>? onPong;

    public async Task ConnectAsync(CancellationToken token)
    {
        ConnectionThread = new Thread(update)
        {
            IsBackground = true,
            Name = "Server Connection Thread"
        };

        try
        {
            // Connect to the server.
            await Connection.ConnectAsync(new Uri(Connector.Provider.EndpointConfiguration.WebsocketUri), token);
            ConnectionThread.Start();
        }
        catch (Exception e)
        {
            // Set the state to failed.
            Logger.Error(e, "Failed to connect to the server.");
            Connector.State.Value = ServerConnectionState.Failed;
            await DisposeAsync();
        }
    }

    public void Queue(Packet packet)
    {
        // Serialize the packet.
        var json = JsonConvert.SerializeObject(packet);

        // Send the packet to the server.
        Queue(Encoding.UTF8.GetBytes(json));
    }

    public void Queue(byte[] buffer)
    {
        // Enqueue the message.
        messageQueue.Enqueue(buffer);
    }

    public async Task Invoke(byte[] buffer)
    {
        await Connection.SendAsync(buffer, WebSocketMessageType.Binary, true, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        // Cancel the receive thread.
        if (!Connector.CancellationToken.IsCancellationRequested)
            Connector.CancellationToken.Cancel();

        ConnectionThread = null;

        // Clear the message queue.
        messageQueue.Clear();

        // Close the connection if it is open.
        if (Connection.State == WebSocketState.Open)
            await Connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed.", cancellationToken);

        // Dispose the connection.
        Connection.Dispose();
        Closed?.Invoke(null);
    }

    public async Task<long> Ping()
    {
        var stopwatch = new Stopwatch();
        
        expectingPong = true;
        stopwatch.Start();
        Queue(new PingPacket());
        
        return await Task.Run(() =>
        {
            while (expectingPong)
            {
                Thread.Sleep(1);
            }

            return stopwatch.ElapsedMilliseconds;
        });
    }
    private Task? receiveTask;

    private void update()
    {
        while (true)
        {
            if (Connection.State != WebSocketState.Open)
            {
                Closed?.Invoke(null);
                break;
            }

            if (receiveTask == null || receiveTask.IsCompleted)
                receiveTask = receive(cancellationToken);

            lock (messageQueue)
            {
                while (messageQueue.Count > 0)
                    sendAsync(cancellationToken).Wait(cancellationToken);
            }
        }
    }

    private async Task receive(CancellationToken token)
    {
        if (Connection.State != WebSocketState.Open || token.IsCancellationRequested)
            return;

        var message = "";
        var endOfMessage = false;

        while (!endOfMessage)
        {
            var buffer = Configuration.Default.MemoryAllocator.Allocate<byte>(128);
            var result = await Connection.ReceiveAsync(buffer.Memory, token);

            // Append the message to the buffer.
            message += Encoding.UTF8.GetString(buffer.Memory.Span).TrimEnd('\0');
            buffer.Dispose();

            endOfMessage = result.EndOfMessage;
        }

        if (string.IsNullOrEmpty(message))
            return;

        RawMessageReceived?.Invoke(Encoding.UTF8.GetBytes(message));

        try
        {
            var packet = JsonConvert.DeserializeObject<Packet>(message);
            packet!.RawData = message;

            PacketReceived?.Invoke(packet);
        }
        catch (JsonReaderException)
        {
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to deserialize packet.");
        }
    }

    private async Task sendAsync(CancellationToken token)
    {
        // Check if the message queue is empty.
        if (messageQueue.Count == 0)
            return;

        // Dequeue the message.
        var message = messageQueue.Dequeue();

        // Send the message to the server.
        try
        {
            await Connection.SendAsync(message, WebSocketMessageType.Binary, true, token);
        }
        catch (OperationCanceledException)
        {
            // The connection was closed.
        }
    }
}
