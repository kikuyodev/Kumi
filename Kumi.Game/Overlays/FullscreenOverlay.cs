using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Kumi.Game.Overlays;

public abstract partial class FullscreenOverlay : KumiFocusedOverlayContainer
{
    [Resolved]
    protected IAPIConnectionProvider API { get; private set; } = null!;

    protected override Container<Drawable> Content => content;

    private readonly Container content;

    private readonly Container mainContent;
    
    protected FullscreenOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Width = 0.8f;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Padding = new MarginPadding { Vertical = 8 };
        
        Masking = true;

        EdgeEffect = new EdgeEffectParameters
        {
            Colour = Color4.Black.Opacity(0),
            Type = EdgeEffectType.Shadow,
            Hollow = true,
            Radius = 6
        };
        
        AddRangeInternal(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.05f)
            },
            content = new Container
            {
                RelativeSizeAxes = Axes.Both
            }
        });
    }
    
    protected override void PopIn()
    {
        FadeEdgeEffectTo(0.25f, 200, Easing.OutQuint);
        this.FadeIn(200, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        FadeEdgeEffectTo(0f, 200, Easing.OutQuint);
        this.FadeOut(200, Easing.OutQuint);
    }
}
