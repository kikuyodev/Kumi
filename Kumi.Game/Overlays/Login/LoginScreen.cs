using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace Kumi.Game.Overlays.Login;

public partial class LoginScreen : Screen
{
    private KumiTextBox username = null!;
    private KumiTextBox password = null!;
    
    public LoginScreen()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }
    
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(0, 8),
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = "SIGN IN",
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 14)
                },
                username = new KumiTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 24,
                    PlaceholderText = "Username",
                    TabbableContentContainer = this
                },
                password = new KumiPasswordTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 24,
                    PlaceholderText = "Password",
                    TabbableContentContainer = this
                },
                new KumiButton
                {
                    Text = "Sign in",
                    Important = true
                },
                new KumiCheckbox
                {
                    LabelText = "Remember me"
                },
                new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Margin = new MarginPadding
                    {
                        Top = 8
                    },
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = "Don't have an account?",
                            Font = KumiFonts.GetFont(size: 14),
                            Colour = Color4Extensions.FromHex("666")
                        },
                        new ClickableContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Child = new SpriteText
                            {
                                Text = "Register one now!",
                                Font = KumiFonts.GetFont(size: 14),
                                Colour = Color4Extensions.FromHex("80DFFF")
                            },
                        }
                    }
                }
            }
        };
    }

    public override bool AcceptsFocus => true;

    protected override bool OnClick(ClickEvent e) => true;

    protected override void OnFocus(FocusEvent e)
    {
        Schedule(() =>
        {
            GetContainingInputManager().ChangeFocus(username);
        });
    }
}
