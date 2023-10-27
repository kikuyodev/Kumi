using osu.Framework;

namespace Kumi.Tests;

public static class Program
{
    public static void Main()
    {
        using var host = Host.GetSuitableDesktopHost("KumiTests");
        using var game = new KumiTestBrowser();

        host.Run(game);
    }
}
