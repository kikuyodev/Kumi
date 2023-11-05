using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game.Screens;

public partial class KumiScreen : Screen
{
    protected BackgroundScreen CurrentBackground { get; private set; } = null!;

    public virtual float ParallaxAmount => 0.015f;
    public virtual float BlurAmount => 0f;
    
    public KumiScreen()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        LoadComponent(CurrentBackground = CreateBackground());
    }
    
    [Resolved]
    private BackgroundScreenStack stack { get; set; } = null!;

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        
        stack.Push(CurrentBackground);
        
        CurrentBackground.BackgroundStack.Parallax.Amount = ParallaxAmount;
        CurrentBackground.BackgroundStack.BlurAmount = BlurAmount;
    }

    public virtual BackgroundScreen CreateBackground() => new BlackBackground();
}
