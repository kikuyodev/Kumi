using Kumi.Game.Graphics;
using Kumi.Game.Overlays;
using Kumi.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osuTK.Graphics;

namespace Kumi.Game.Tests;

public abstract partial class KumiScreenTestScene : KumiTestScene, IOverlayManager
{
    protected readonly KumiScreenStack ScreenStack;

    private readonly Container content;
    private readonly Container overlayContent;
    
    protected override Container<Drawable> Content => content;

    protected KumiScreenTestScene()
    {
        base.Content.AddRange(new Drawable[]
        {
            ScreenStack = new KumiScreenStack
            {
                RelativeSizeAxes = Axes.Both
            },
            content = new Container { RelativeSizeAxes = Axes.Both },
            overlayContent = new Container { RelativeSizeAxes = Axes.Both }
        });
    }

    protected void LoadScreen(KumiScreen screen)
        => ScreenStack.Push(screen);

    [SetUpSteps]
    public virtual void SetupSteps()
        => addExitAllScreensStep();
    
    private void addExitAllScreensStep()
    {
        AddUntilStep("Exit all screens", () =>
        {
            if (ScreenStack.CurrentScreen == null)
                return true;
            
            ScreenStack.Exit();
            return false;
        });
    }

    public IDisposable RegisterBlockingOverlay(OverlayContainer container)
    {
        overlayContent.Add(container);
        return new InvokeOnDisposal(() => content.Expire());
    }

    public void ShowBlockingOverlay(OverlayContainer container)
        => Schedule(() => ScreenStack.FadeColour(Colours.Gray(0.5f), 500, Easing.OutQuint));

    public void HideBlockingOverlay(OverlayContainer container)
        => Schedule(() => ScreenStack.FadeColour(Color4.White, 500, Easing.OutQuint));
}
