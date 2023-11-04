using Kumi.Game.Graphics.Containers;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Graphics.Backgrounds;

/// <summary>
/// A stack of backgrounds that can be displayed by a <see cref="BackgroundScreen"/>, also there
/// for handling transitions between backgrounds; as well as any effects that may be applied to
/// them.
/// </summary>
public partial class BackgroundStack : CompositeDrawable
{
    public ParallaxContainer Parallax { get; private set; }
    
    /// <summary>
    /// The current background that is being displayed.
    /// </summary>
    public Background? CurrentBackground { get; private set; }

    private float blurAmount;

    public float BlurAmount
    {
        get => blurAmount;
        set
        {
            if (blurAmount == value)
                return;
            
            blurAmount = value;
            blurContainer?.FinishTransforms();
            blurContainer?.BlurTo(new Vector2(blurAmount), ParallaxContainer.PARALLAX_DURATION, Easing.OutQuint);
        }
    }

    private readonly BufferedContainer? blurContainer;
    
    public BackgroundStack()
    {
        AddInternal(blurContainer = new BufferedContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = Parallax = new ParallaxContainer
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }
        });
    }

    public void Push(Background background, int duration = 1000, BackgroundTransitionType type = BackgroundTransitionType.Fade, Easing easing = Easing.In)
    {
        if (background.Equals(CurrentBackground))
            return;
        
        if (CurrentBackground == null)
        {
            SetBackgroundImmediately(background);
            return;
        }
        
        switch (type)
        {
            case BackgroundTransitionType.Fade:
                background.Alpha = 0;
                Parallax.Add(background);
                
                background.FadeIn(duration, easing);
                CurrentBackground.FadeOut(duration, easing);
                CurrentBackground = background;
                break;
        }
    }

    /// <summary>
    /// Immediately sets the background to the given background, without any transitions.
    /// </summary>
    /// <param name="background"></param>
    public void SetBackgroundImmediately(Background background)
    {
        Parallax.Child = CurrentBackground = background;
    }
}

public enum BackgroundTransitionType
{
    Fade,
}