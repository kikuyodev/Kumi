using Kumi.Game;

namespace Kumi.Desktop;

public partial class KumiDesktopGame : KumiGame
{
    protected override void LoadComplete()
    {
        LoadComponent(new DiscordPresence());
        
        base.LoadComplete();
    }
}
