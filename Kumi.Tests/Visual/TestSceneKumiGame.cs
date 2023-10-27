using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace Kumi.Tests.Visual;

[TestFixture]
public partial class TestSceneKumiGame : KumiTestScene
{
    private KumiGame game = null!;

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        game = new KumiGame();
        game.SetHost(host);
        
        AddGame(game);
    }
}
