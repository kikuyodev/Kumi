using Kumi.Game.Screens;
using osu.Framework.Graphics;

namespace Kumi.Game.Tests;

public abstract partial class KumiScreenTestScene : KumiTestScene
{
    protected readonly KumiScreenStack ScreenStack;

    private KumiScreen? screen;

    protected KumiScreenTestScene()
    {
        base.Content.Add(ScreenStack = new KumiScreenStack
        {
            RelativeSizeAxes = Axes.Both
        });
    }
    
    protected void PushScreen(KumiScreen newScreen) => AddStep("push screen", () => ScreenStack.Push(screen = newScreen));

    protected void WaitForScreenLoad() => AddUntilStep("Screen is loaded", () => screen?.IsLoaded ?? false);
}
