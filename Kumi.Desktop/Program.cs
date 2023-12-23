using Kumi.Game;
using osu.Framework;
using osu.Framework.Development;
using Squirrel;

namespace Kumi.Desktop;

public static class Program
{
#if DEBUG
    public const string APP_NAME = "Kumi Development";
#else
    public const string APP_NAME = "Kumi";
#endif

    public static void Main(string[] args)
    {
        using var host = Host.GetSuitableDesktopHost(APP_NAME);
        using var game = new KumiGame();

        handleSquirrelEvents();
        host.Run(game);
    }

    private static void handleSquirrelEvents()
    {
        SquirrelAwareApp.HandleEvents(
            onInitialInstall: (_, tools) =>
            {
                tools.CreateShortcutForThisExe();
                tools.CreateUninstallerRegistryEntry();
            },
            onAppUninstall: (_, tools) =>
            {
                tools.RemoveShortcutForThisExe();
                tools.RemoveUninstallerRegistryEntry();
            },
            onAppUpdate: (_, tools) =>
            {
                tools.CreateUninstallerRegistryEntry();
            }
        );
    }
}
