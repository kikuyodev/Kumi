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

    private readonly Color4 backgroundColor = Colours.Gray(0.07f);
    private readonly Color4 inputErrorColour = Colours.RED;
    
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

        background.FlashColour(Colours.Gray(0.15f), 200, Easing.OutQuint);
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
        Colour = Colours.GRAY_6,
        Font = KumiFonts.GetFont(italics: true),
        Anchor = Anchor.CentreLeft,
        Origin = Anchor.CentreLeft,
    };

    protected override Caret CreateCaret() => new BasicTextBox.BasicCaret
    {
        CaretWidth = 2,
        SelectionColour = Colours.CYAN_ACCENT.Opacity(0.4f)
    };
}
