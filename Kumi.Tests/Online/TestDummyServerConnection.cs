using Kumi.Game.Online.Server;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Testing;

namespace Kumi.Tests.Online;

[HeadlessTest]
public partial class TestDummyServerConnection : KumiTestScene
{
    [Test]
    public void TestCreateConnection()
    {
        var connector = connect();

        AddStep("login to account", () =>
        {
            API.Login("username", "password");
        });

        AddAssert("is started", () => connector.Started);
        AddUntilStep("connection is made, and connected", () => connector.CurrentConnection != null && connector.CurrentConnection.IsConnected);

        AddStep("stop the connector", () =>
        {
            API.Logout();
        });
    }

    [Test]
    public void TestSendingAndRecievingData()
    {
        var connector = connect();
        var caughtPackets = new List<TestPacket>();

        AddStep("login to account", () =>
        {
            API.Login("username", "password");
        });

        connector.RegisterPacketHandler<TestPacket>(ServerPacketOpCode.Hello, packet =>
        {
            caughtPackets.Add(packet);
        });

        AddUntilStep("connection is made, and connected", () => connector.CurrentConnection != null && connector.CurrentConnection.IsConnected);

        AddStep("send hello packet", () =>
        {
            connector.CurrentConnection!.Queue(new TestPacket());
        });

        AddUntilStep("wait for packet", () => caughtPackets.Count > 0);
        AddAssert("packet opcode is hello", () => caughtPackets[0].OpCode == ServerPacketOpCode.Hello);
        AddAssert("packet data is correct", () => caughtPackets[0].Data.SequenceEqual(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 }));
    }

    private IServerConnector connect()
    {
        var connector = API.GetServerConnector();

        AddStep("start the connector", () =>
        {
            connector.Start();
        });

        return connector;
    }

    internal class TestPacket : ServerPacket<byte[]>
    {
        public TestPacket()
        {
            OpCode = ServerPacketOpCode.Hello;
            Data = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 };
        }
    }
}
