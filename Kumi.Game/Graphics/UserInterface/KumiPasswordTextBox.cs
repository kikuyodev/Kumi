using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiPasswordTextBox : KumiTextBox
{
    protected override Drawable GetDrawableCharacter(char c) => new BasicTextBox.FallingDownContainer
    {
        AutoSizeAxes = Axes.Both,
        Child = new SpriteText
        {
            Text = "•",
            Font = KumiFonts.GetFont(size: CalculatedTextSize)
        }
    };
}
