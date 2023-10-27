using osu.Framework;

namespace Kumi.Desktop;

public static class Program
{
    public static void Main(string[] args)
    {
        using var host = Host.GetSuitableDesktopHost("KumiTests");
        using var game = new KumiGame();

        host.Run(game);
    }
}
