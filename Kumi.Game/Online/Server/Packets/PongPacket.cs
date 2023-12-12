namespace Kumi.Game.Online.Server.Packets;

public class PongPacket : Packet
{
    public override OpCode OpCode => OpCode.Pong;
}
