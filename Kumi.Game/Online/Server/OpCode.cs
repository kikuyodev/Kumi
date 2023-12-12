namespace Kumi.Game.Online.Server;

public enum OpCode
{
    Dispatch = 0x00,
    Hello = 0x01,
    Identify = 0x02,
    
    Ping = 0x09,
    Pong = 0x0A,
}
