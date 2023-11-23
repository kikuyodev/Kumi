namespace Kumi.Game.Online.API;

public class APIRequestException : Exception
{
    public APIError? Error { get; set; }

    public APIRequestException(APIError? error, string? message)
        : base(message ?? error?.Message ?? "An API request has failed, but no error message was provided.")
    {
        Error = error;
    }
}
