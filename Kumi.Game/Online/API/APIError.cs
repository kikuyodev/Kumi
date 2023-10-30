namespace Kumi.Game.Online.API;

public class APIError : IHasStatus
{
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}
