using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Screens.Backgrounds;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Testing;

namespace Kumi.Tests.Visual.Backgrounds;

[TestFixture]
public partial class TestVisualBackgroundScreen : KumiTestScene
{
    private BackgroundScreenStack stack = null!;
    private BackgroundScreen backgroundScreen = null!;
    
    [SetUpSteps]
    public void SetUpSteps()
    {
        AddStep("create background stack", () => Child = stack = new BackgroundScreenStack());
        AddStep("create background screen", () => stack.Push(backgroundScreen = new TestBackgroundScreen()));
        AddUntilStep("wait until the screen is loaded", () => backgroundScreen.IsLoaded);
        AddUntilStep("wait until the current screen is changed", () => backgroundScreen.IsCurrentScreen());
    }

    private partial class TestBackgroundScreen : BackgroundScreen
    {
        private Background background;
        private Background secondBackground;
        public TestBackgroundScreen()
        {
            SetBackgroundImmediately(background = new Background());
            secondBackground = new Background();
        }
        
        [BackgroundDependencyLoader]
        private void load(IRenderer renderer, GameHost host)
        {
            background.SetBackground(TestResources.OpenTexture(renderer, host, "Images/Backgrounds/town.jpg"));
            secondBackground.SetBackground(TestResources.OpenTexture(renderer, host, "Images/Backgrounds/town2.jpg"));
            
            Scheduler.AddDelayed(() =>
            {
                this.BackgroundStack.Push(secondBackground);
            }, 5000);
        }
    }
}