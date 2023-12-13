namespace Kumi.Game.Online.Server.Packets;

public class HelloPacket : Packet<HelloPacket.HelloData>
{
    public override OpCode OpCode => OpCode.Hello;

    public class HelloData
    {
    }
}
