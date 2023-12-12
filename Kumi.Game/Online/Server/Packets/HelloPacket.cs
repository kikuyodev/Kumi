namespace Kumi.Game.Online.Server.Packets;

public class HelloPacket : Packet<HelloPacket.HelloData>
{
    public OpCode OpCode => OpCode.Hello;
    
    public class HelloData
    {
    }
}
