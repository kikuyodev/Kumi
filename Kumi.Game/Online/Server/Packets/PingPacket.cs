namespace Kumi.Game.Online.Server.Packets;

public class PingPacket : Packet
{
    public override OpCode OpCode => OpCode.Ping;
}
