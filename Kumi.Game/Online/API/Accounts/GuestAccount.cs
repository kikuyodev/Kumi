namespace Kumi.Game.Online.API.Accounts;

public class GuestAccount : APIAccount
{
    public GuestAccount()
    {
        Id = DEFAULT_SYSTEM_ID;
        Username = "Guest";
    }
}
