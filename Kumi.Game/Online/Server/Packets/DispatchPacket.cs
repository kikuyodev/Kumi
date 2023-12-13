namespace Kumi.Game.Online.Server.Packets;

public class DispatchPacket<T> : Packet<T> 
    where T : class
{
    public override OpCode OpCode => OpCode.Dispatch;
}
