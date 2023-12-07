using Kumi.Game.Screens.Play;
using Kumi.Game.Tests;
using NUnit.Framework;

namespace Kumi.Tests.Visual.Play;

public partial class ChartLoaderDisplayTestScene : KumiTestScene
{
    private ChartLoaderDisplay display = null!;

    [Test]
    public void TestDisplay()
    {
        AddStep("clear", Clear);
        AddStep("add display", () =>
        {
            Add(display = new ChartLoaderDisplay(TestResources.CreateChartSet(1).Charts[0]));
        });
        AddToggleStep("toggle ready", v => display.IsReady.Value = v);
    }
}
