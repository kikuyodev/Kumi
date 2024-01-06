using Kumi.Game.Graphics;
using Kumi.Game.Input;
using Kumi.Game.Online.API;
using Kumi.Game.Overlays.Taskbar;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Overlays;

public partial class TaskbarOverlay : OverlayContainer, IKeyBindingHandler<GlobalAction>
{
    public const float HEIGHT = 48;

    private FillFlowContainer leftFlow = null!;
    private FillFlowContainer rightFlow = null!;
    private Container overlayContent = null!;

    private LoginOverlay loginOverlay = null!;

    private bool initialShow;

    protected override bool StartHidden => true;

    public readonly IBindable<OverlayActivation> OverlayActivation = new Bindable<OverlayActivation>(Overlays.OverlayActivation.Any);

    public override bool PropagateNonPositionalInputSubTree => OverlayActivation.Value != Overlays.OverlayActivation.Disabled;

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;

    [BackgroundDependencyLoader(true)]
    private void load(KumiGame? kumiGame)
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

        addButton(new TaskbarIconButton { Alignment = TaskbarButtonAlignment.Left, Icon = FontAwesome.Solid.Home });
        addButton(new TaskbarSettingButton { Alignment = TaskbarButtonAlignment.Left });

        addButton(new TaskbarListingButton { Alignment = TaskbarButtonAlignment.Right });
        addButton(new TaskbarChatButton { Alignment = TaskbarButtonAlignment.Right });
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
        addButton(new TaskbarNotificationButton { Alignment = TaskbarButtonAlignment.Right });

        api.State.BindValueChanged(state =>
        {
            switch (state.NewValue)
            {
                case APIState.Online:
                    loginOverlay.State.Value = Visibility.Hidden;
                    break;
            }
        });

        if (kumiGame != null)
            OverlayActivation.BindTo(kumiGame.OverlayActivation);
    }

    protected override void UpdateState(ValueChangedEvent<Visibility> state)
    {
        var blocked = OverlayActivation.Value == Overlays.OverlayActivation.Disabled;

        if (state.NewValue == Visibility.Visible && blocked)
        {
            State.Value = Visibility.Hidden;
            return;
        }

        base.UpdateState(state);
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
        this.MoveToY(-HEIGHT, 200, Easing.OutQuint);
    }

    private void addButton(TaskbarButton button, Action? action = null)
    {
        if (action != null)
            button.Action = action;

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

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (OverlayActivation.Value == Overlays.OverlayActivation.Disabled)
            return false;

        switch (e.Action)
        {
            case GlobalAction.ToggleTaskbar:
                ToggleVisibility();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }
}
