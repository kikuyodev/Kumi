using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Overlays.Control;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osuTK.Graphics;

namespace Kumi.Game.Overlays;

public partial class ControlOverlay : KumiFocusedOverlayContainer, INotificationManager
{
    public const float WIDTH = 336;

    public IEnumerable<Notification> AllNotifications
        => IsLoaded ? notificationSection.Notifications : Enumerable.Empty<Notification>();

    private readonly BindableInt unreadCount = new BindableInt();
    public IBindable<int> UnreadCount => unreadCount;
    
    private NotificationSection notificationSection = null!;
    private Container content = null!;

    public ControlOverlay()
    {
        RelativeSizeAxes = Axes.Y;
        Width = WIDTH;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Padding = new MarginPadding(4);
    }
    
    [BackgroundDependencyLoader]
    private void load()
    {
        Child = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            EdgeEffect = new EdgeEffectParameters
            {
                Hollow = true,
                Radius = 8,
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0f)
            },
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f)
                },
                new BasicScrollContainer
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Vertical,
                            AutoSizeAxes = Axes.Y,
                            RelativeSizeAxes = Axes.X,
                            Children = new Drawable[]
                            {
                                notificationSection = new NotificationSection()
                            }
                        }
                    }
                }
            }
        };
    }
    
    protected override void PopIn()
    {
        this.MoveToX(0, 600, Easing.OutQuint);
        content.FadeTo(1, 600, Easing.OutQuint);
        content.FadeEdgeEffectTo(0.25f, 600, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        markAllRead();

        this.MoveToX(WIDTH, 600, Easing.OutQuint);
        content.FadeTo(0, 600, Easing.OutQuint);
        content.FadeEdgeEffectTo(0, 600, Easing.OutQuint);
    }

    public void Post(Notification notification)
    {
        Logger.Log($"※ {notification.Message ?? notification.Header ?? "Notification"}", LoggingTarget.Information);
        
        notification.Closed += notificationClosed;
        
        notificationSection.Add(notification);
        updateCount();
    }

    private void notificationClosed()
    {
        Schedule(updateCount);
    }

    private void markAllRead()
    {
        notificationSection.MarkAllRead();
        updateCount();
    }

    private void updateCount()
    {
        unreadCount.Value = notificationSection.UnreadCount;
    }
}
