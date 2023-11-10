using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace Kumi.Game.Overlays.Login;

public partial class LoginPanel : Container
{
    public override RectangleF BoundingBox => hidden ? RectangleF.Empty : base.BoundingBox;

    private LoginScreenStack formStack = null!;
    private LoginScreen? loginScreen;
    
    private bool hidden;

    public bool Hidden
    {
        get => hidden;
        set
        {
            hidden = value;
            Invalidate(Invalidation.MiscGeometry);
        }
    }

    public LoginPanel()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4Extensions.FromHex("1A1A1A")
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4Extensions.FromHex("F94826"),
                Padding = new MarginPadding(4),
                Children = new Drawable[]
                {
                    new Circle
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 3,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                    },
                    new Circle
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 3,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                    },
                }
            },
            formStack = new LoginScreenStack
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding
                {
                    Vertical = 16,
                    Horizontal = 12
                },
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        formStack.Push(loginScreen = new LoginScreen());
    }

    public override bool AcceptsFocus => true;

    protected override bool OnClick(ClickEvent e) => true;

    protected override void OnFocus(FocusEvent e)
    {
        if (loginScreen != null)
            GetContainingInputManager().ChangeFocus(loginScreen);
            
        base.OnFocus(e);
    }
}
