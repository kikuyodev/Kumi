using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiTextBox : TextBox
{
    protected override float LeftRightPadding => 12;

    private readonly Box background;

    private readonly Color4 backgroundColor = Color4Extensions.FromHex("121212");
    private readonly Color4 inputErrorColour = Color4Extensions.FromHex("732639");
    
    public KumiTextBox()
    {
        Masking = true;
        CornerRadius = 5;
        
        AddRangeInternal(new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1,
                Colour = backgroundColor,
            },
        });

        TextContainer.Height = 0.75f;
    }

    protected override void NotifyInputError()
    {
        background.FlashColour(inputErrorColour, 200, Easing.OutQuint);
    }

    protected override void OnTextCommitted(bool textChanged)
    {
        base.OnTextCommitted(textChanged);

        background.FlashColour(Color4Extensions.FromHex("262626"), 200, Easing.OutQuint);
    }

    protected override Drawable GetDrawableCharacter(char c) => new BasicTextBox.FallingDownContainer
    {
        AutoSizeAxes = Axes.Both,
        Child = new SpriteText
        {
            Text = c.ToString(),
            Font = KumiFonts.GetFont(size: 14)
        }
    };

    protected override SpriteText CreatePlaceholder() => new BasicTextBox.FadingPlaceholderText
    {
        Colour = Color4Extensions.FromHex("666"),
        Font = KumiFonts.GetFont(italics: true),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
    };

    protected override Caret CreateCaret() => new BasicTextBox.BasicCaret
    {
        CaretWidth = 2,
        SelectionColour = Color4Extensions.FromHex("00BFFF").Opacity(0.4f)
    };
}
