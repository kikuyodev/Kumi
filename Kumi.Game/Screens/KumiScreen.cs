using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game.Screens;

public partial class KumiScreen : Screen
{
    protected BackgroundScreen CurrentBackground { get; private set; } = null!;

    public readonly Bindable<OverlayActivation> OverlayActivation = null!;

    protected virtual OverlayActivation InitialOverlayActivation => Overlays.OverlayActivation.Any;
    
    public virtual float ParallaxAmount => 0.015f;
    public virtual float BlurAmount => 0f;
    public virtual float DimAmount => 0.95f;
    
    public virtual bool AllowBackButton => true;
    public virtual bool HideOverlaysOnEnter => true;

    public KumiScreen()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        
        OverlayActivation = new Bindable<OverlayActivation>(InitialOverlayActivation);
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
        CurrentBackground.BackgroundStack.DimAmount = DimAmount;
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);

        CurrentBackground.MakeCurrent();

        CurrentBackground.BackgroundStack.Parallax.Amount = ParallaxAmount;
        CurrentBackground.BackgroundStack.BlurAmount = BlurAmount;
        CurrentBackground.BackgroundStack.DimAmount = DimAmount;
    }

    public virtual BackgroundScreen CreateBackground() => new BlackBackground();
}
