using Kumi.Game.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;

namespace Kumi.Game.Overlays;

public partial class OnlineOverlay : FullscreenOverlay
{
    protected override Container<Drawable> Content => content;

    private readonly Container content;

    public OnlineOverlay(bool shouldScroll = false)
    {
        Container<Drawable> mainContent = shouldScroll
                              ? new KumiScrollContainer { ScrollbarVisible = false }
                              : new Container();

        mainContent.RelativeSizeAxes = Axes.Both;
        
        mainContent.Add(new KumiContextMenuContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = content = new Container
                {
                    RelativeSizeAxes = Axes.Both
                }
            }
        });
        
        base.Content.Add(mainContent);
    }
}
