using Kumi.Game.Graphics;
using Kumi.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Kumi.Game.Overlays.Taskbar;

public partial class TaskbarNotificationButton : TaskbarButton, IKeyBindingHandler<GlobalAction>
{
    private readonly IBindable<int> notificationCount = new BindableInt();

    private NotificationBell notificationBell = null!;
    private ControlOverlay? controlOverlay;

    [BackgroundDependencyLoader(true)]
    private void load(INotificationManager? notificationManager)
    {
        controlOverlay = notificationManager as ControlOverlay;

        if (notificationManager != null)
            notificationCount.BindTo(notificationManager.UnreadCount);

        notificationCount.ValueChanged += count =>
        {
            if (count.NewValue == 0)
                notificationBell.FadeOut(200, Easing.OutQuint);
            else
            {
                notificationBell.NotificationCount = count.NewValue;
                notificationBell.FadeIn(200, Easing.OutQuint);
            }
        };

        Action = toggleOverlay;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        MainContent.AutoSizeDuration = 250;
        MainContent.AutoSizeEasing = Easing.OutQuint;
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        switch (e.Action)
        {
            case GlobalAction.ToggleNotifications:
                toggleOverlay();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    private void toggleOverlay()
    {
        if (controlOverlay?.State.Value == Visibility.Hidden)
            controlOverlay.Show();
        else
            controlOverlay?.Hide();
    }

    protected override Drawable CreateContent()
        => new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Y,
            AutoSizeAxes = Axes.X,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(12, 0),
            Children = new Drawable[]
            {
                notificationBell = new NotificationBell
                {
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Child = new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Solid.Bars,
                        Size = new(20)
                    }
                }
            }
        };

    private partial class NotificationBell : FillFlowContainer
    {
        private readonly SpriteText countText;

        private int notificationCount;

        public int NotificationCount
        {
            get => notificationCount;
            set
            {
                if (notificationCount == value)
                    return;

                notificationCount = value;
                countText.Text = value.ToString("#,0");
            }
        }

        public NotificationBell()
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(4, 0);

            Children = new Drawable[]
            {
                new Container
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AutoSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = 4,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colours.GRAY_C,
                                },
                                new Container
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    AutoSizeAxes = Axes.Both,
                                    Padding = new MarginPadding
                                    {
                                        Horizontal = 8,
                                    },
                                    Child = countText = new SpriteText
                                    {
                                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12),
                                        Colour = Colours.Gray(0.1f),
                                        Text = "0"
                                    }
                                }
                            }
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Child = new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Solid.Bell,
                        Size = new(16)
                    }
                }
            };
        }
    }
}
