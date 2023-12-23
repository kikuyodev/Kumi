using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Accounts;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Chat;

public partial class DrawableChatUsername : Container
{
    public Color4 AccentColour { get; init; }
    
    private readonly APIAccount account;
    private readonly SpriteText drawableText;

    private Drawable colouredDrawable = null!;

    public DrawableChatUsername(APIAccount account)
    {
        this.account = account;

        AutoSizeAxes = Axes.Both;
        
        drawableText = new SpriteText
        {
            Text = account.Username,
            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
            Colour = Colours.GRAY_C
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (!account.Groups.Any())
        {
            Add(colouredDrawable = drawableText);
        }
        else
        {
            Add(new Container
            {
                AutoSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 3,
                EdgeEffect = new EdgeEffectParameters
                {
                    Roundness = 1,
                    Radius = 1,
                    Colour = Color4.Black.Opacity(0.3f),
                    Offset = new Vector2(0, 1),
                    Type = EdgeEffectType.Shadow
                },
                Child = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 3,
                    Children = new[]
                    {
                        colouredDrawable = new Box
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = 4, Right = 4, Bottom = 1, Top = -2 },
                            Child = drawableText
                        }
                    }
                }
            });

            drawableText.Colour = Colours.GRAY_1;
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        colouredDrawable.Colour = AccentColour;
    }

    protected override bool OnHover(HoverEvent e)
    {
        colouredDrawable.FadeColour(AccentColour.Lighten(0.5f), 30, Easing.OutQuint);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);
        colouredDrawable.FadeColour(AccentColour, 200, Easing.OutQuint);
    }
}
