using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Menu;

public partial class MenuButton : ClickableContainer
{
    private const float width = 100f;

    private readonly string text;
    private readonly Colour4 accentColour;
    private readonly Texture texture;

    public MenuButton(string text, Colour4 accentColour, Texture texture, Action onClick)
    {
        this.text = text.ToUpperInvariant();
        this.accentColour = accentColour;
        this.texture = texture;
        Action = onClick;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = width;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Masking = true;
        CornerRadius = 5;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.05f),
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FillMode = FillMode.Fill,
                        Texture = texture,
                        Alpha = 0.25f
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(accentColour.Opacity(0.9f), accentColour.Opacity(0.1f))
                    }
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4),
                Children = createVerticalText(c =>
                {
                    c.Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 28);
                })
            }
        };
    }

    private float originalHeight;

    protected override bool OnHover(HoverEvent e)
    {
        originalHeight = Height;
        this.ResizeHeightTo(Height * 1.1f, 200, Easing.OutQuint);
        return base.OnHover(e);
    }
    
    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.ResizeHeightTo(originalHeight, 200, Easing.OutQuint);
        base.OnHoverLost(e);
    }

    private Drawable[] createVerticalText(Action<SpriteText> textModifier)
    {
        var drawables = new Drawable[text.Length];

        for (var i = 0; i < text.Length; i++)
        {
            var letter = text[i];
            var spriteText = new SpriteText
            {
                Text = letter.ToString(),
                Colour = Color4.White,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };

            textModifier(spriteText);
            drawables[i] = spriteText;
        }
        
        return drawables;
    }
}
