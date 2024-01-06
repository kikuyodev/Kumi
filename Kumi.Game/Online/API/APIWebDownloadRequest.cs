using osu.Framework.IO.Network;

namespace Kumi.Game.Online.API;

public abstract class APIDownloadRequest : APIRequest
{
    public string Filename = string.Empty;

    public new event APIWebRequest.APIRequestSucceeded<string>? Success;
    public new event Action<long, long>? DownloadProgress;

    protected APIDownloadRequest()
    {
        base.Success += () => Success?.Invoke(Filename!);
    }

    protected override WebRequest CreateWebRequest()
    {
        var file = Path.GetTempFileName();
        
        File.Move(file, Filename = Path.ChangeExtension(file, ".tmp"));

        var request = new FileWebRequest(Filename, Uri);
        request.DownloadProgress += (current, total) => DownloadProgress?.Invoke(current, total);

        return request;
    }
}
