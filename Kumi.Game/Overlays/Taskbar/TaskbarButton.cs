using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Taskbar;

public abstract partial class TaskbarButton : Button
{
    private Box hoverBox = null!;

    protected Container MainContent = null!;

    public required TaskbarButtonAlignment Alignment { get; init; }
    
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;
        
        Children = new Drawable[]
        {
            hoverBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White.Opacity(0.1f),
                AlwaysPresent = true,
                Alpha = 0
            },
            MainContent = new Container
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Padding = new MarginPadding { Horizontal = 16 },
                Child = CreateContent()
            }
        };
        
        Enabled.BindValueChanged(e =>
        {
            this.FadeColour(e.NewValue ? Color4.White : Color4.White.Darken(0.5f), 100, Easing.OutQuint);
        }, true);
    }
    
    protected abstract Drawable CreateContent();

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeIn(100, Easing.OutQuint);
        return base.OnHover(e);
    }
    
    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeOut(100, Easing.OutQuint);
        base.OnHoverLost(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
            hoverBox.FlashColour(Color4.White.Opacity(0.5f), 500, Easing.OutQuint);
        
        return base.OnClick(e);
    }
}

public enum TaskbarButtonAlignment
{
    Left,
    Right
}
