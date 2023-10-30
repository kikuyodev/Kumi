namespace Kumi.Game.Online.API.Users;

public class GuestAccount : APIAccount
{
    public GuestAccount()
    {
        Id = DEFAULT_SYSTEM_ID;
        Username = "Guest";
    }
}
