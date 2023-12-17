using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Overlays.Chat;

public partial class MessageItem : CompositeDrawable
{
    private readonly string message;

    public MessageItem(string message)
    {
        this.message = message;
        AutoSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new SpriteText
        {
            Text = message,
            Font = KumiFonts.GetFont(size: 12),
            Colour = Colours.GRAY_C
        };
    }
}
