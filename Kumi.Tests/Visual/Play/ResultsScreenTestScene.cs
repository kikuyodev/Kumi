using Kumi.Game.Screens.Play;
using Kumi.Game.Tests;

namespace Kumi.Tests.Visual.Play;

public partial class ResultsScreenTestScene : KumiScreenTestScene
{
    public override void SetupSteps()
    {
        base.SetupSteps();

        var testScore = TestResources.CreateScoreInfo();
        AddStep("Load results screen", () => LoadScreen(new ResultsScreen(testScore)));
        AddUntilStep("Wait for results screen to load", () => ScreenStack.CurrentScreen is ResultsScreen);
    }
}
