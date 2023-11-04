using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osuTK;

namespace Kumi.Game.Graphics.Containers;

public partial class ParallaxContainer : Container, IRequireHighFrequencyMousePosition
{
    /// <summary>
    /// The amount of milliseconds it takes for the parallax effect to return back to the centre.
    /// </summary>
    public const int PARALLAX_BOUNCEBACK_VALUE = 1250;

    /// <summary>
    /// The duration it takes to move the parallax effect.
    /// </summary>
    public const int PARALLAX_DURATION = 250;
    
    /// <summary>
    /// The current strength of the parallax effect.
    /// </summary>
    public float Amount { get; set; } = 0.015f;

    /// <summary>
    /// Whether the parallax effect is enabled.
    /// </summary>
    public bool Enabled
    {
        get => enabledBindable.Value;
        set => enabledBindable.Value = value;
    }

    public ParallaxContainer()
    {
        AddInternal(content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        });
    }
    
    private Bindable<bool> enabledBindable = new Bindable<bool>(false);
    private readonly Container content;
    protected override Container<Drawable> Content => content;
    
    [BackgroundDependencyLoader]
    private void load()
    {
        enabledBindable.ValueChanged += (v) =>
        {
            if (v.NewValue == false)
            {
                // Set the position to the centre of the container.
                content.MoveTo(Vector2.Zero, PARALLAX_BOUNCEBACK_VALUE, Easing.OutQuint);
            }
            else
            {
                content.ScaleTo(1f + Amount, PARALLAX_BOUNCEBACK_VALUE, Easing.OutQuint);
            }
        };

        // Force a change to the bindable to set the initial state.
        enabledBindable.Value = true;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!Enabled)
            return;
        
        Vector2 mousePos = ToLocalSpace(GetContainingInputManager().CurrentState.Mouse.Position);
        
        // Calculate the amount to move the content by.
        Vector2 amountToMove = (mousePos - DrawSize / 2) * Amount;
        
        // Move the content by the calculated amount.
        content.MoveTo(-amountToMove, PARALLAX_DURATION, Easing.OutQuint);
    }
}
