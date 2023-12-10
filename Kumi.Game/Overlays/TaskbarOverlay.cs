using Kumi.Game.Graphics;
using Kumi.Game.Online.API;
using Kumi.Game.Overlays.Taskbar;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Overlays;

public partial class TaskbarOverlay : OverlayContainer
{
    public const float HEIGHT = 48;

    private FillFlowContainer leftFlow = null!;
    private FillFlowContainer rightFlow = null!;
    private Container overlayContent = null!;
    
    private LoginOverlay loginOverlay = null!;

    private bool initialShow;
    
    protected override bool StartHidden => true;
    
    [Resolved]
    private IAPIConnectionProvider api { get; set;  } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        
        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 48,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colours.Gray(0.05f)
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding
                        {
                            Horizontal = 40
                        },
                        Children = new Drawable[]
                        {
                            leftFlow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.Y,
                                AutoSizeAxes = Axes.X
                            },
                            rightFlow = new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.Y,
                                AutoSizeAxes = Axes.X,
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight
                            }
                        }
                    }
                }
            },
            overlayContent = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding
                {
                    Top = 48,
                    Horizontal = 40
                },
                Children = new Drawable[]
                {
                    loginOverlay = new LoginOverlay
                    {
                        Width = 208,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Margin = new MarginPadding { Top = 4 }
                    }
                }
            }
        };
        
        addButton(new TaskbarIconButton
        {
            Alignment = TaskbarButtonAlignment.Left,
            Icon = FontAwesome.Solid.Home
        });
        
        addButton(new TaskbarUserButton
        {
            Alignment = TaskbarButtonAlignment.Right,
        }, () =>
        {
            if (api.LocalAccount.Value.Id != 0)
            {
                // TODO: Open user profile
                return;
            }
            
            loginOverlay.State.Value = loginOverlay.State.Value == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        });

        api.State.BindValueChanged(state =>
        {
            switch (state.NewValue)
            {
                case APIState.Online:
                    loginOverlay.State.Value = Visibility.Hidden;
                    break;
            }
        });
    }

    protected override void PopIn()
    {
        this.MoveToY(0, 200, Easing.OutQuint);

        if (api.State.Value == APIState.Offline && !initialShow)
        {
            loginOverlay.State.Value = Visibility.Visible;
            initialShow = true;
        }
    }

    protected override void PopOut()
    {
        this.MoveToY(-DrawHeight, 200, Easing.OutQuint);
    }
    
    private void addButton(TaskbarButton button, Action? action = null)
    {
        button.Action = action ?? button.Action;
        
        switch (button.Alignment)
        {
            case TaskbarButtonAlignment.Left:
                leftFlow.Add(button);
                break;
            case TaskbarButtonAlignment.Right:
                rightFlow.Add(button);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
