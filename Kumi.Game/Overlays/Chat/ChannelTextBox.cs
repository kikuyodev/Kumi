using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Overlays.Chat;

public partial class ChannelTextBox : KumiTextBox
{
    public ChannelTextBox()
    {
        TextContainer.Height = 0.6f;
    }

    protected override SpriteText CreatePlaceholder() => new BasicTextBox.FadingPlaceholderText
    {
        Colour = Colours.GRAY_6,
        Font = KumiFonts.GetFont(),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
    };
}
