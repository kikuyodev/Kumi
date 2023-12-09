using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API;
using osu.Framework.Allocation;
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
    
    [Resolved]
    private IAPIConnectionProvider api { get; set;  }
    
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
                    Important = true,
                    BackgroundColour = Colours.ORANGE_ACCENT_LIGHT,
                    Action = () =>
                    {
                        api.Login(username.Text, password.Text);
                    }
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
                            Colour = Colours.GRAY_C
                        },
                        new ClickableContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Child = new SpriteText
                            {
                                Text = "Register one now!",
                                Font = KumiFonts.GetFont(size: 14),
                                Colour = Colours.CYAN_ACCENT_LIGHTER
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
