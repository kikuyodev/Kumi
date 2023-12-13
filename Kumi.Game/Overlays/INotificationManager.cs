using Kumi.Game.Overlays.Control;
using osu.Framework.Allocation;
using osu.Framework.Bindables;

namespace Kumi.Game.Overlays;

[Cached]
public interface INotificationManager
{
    void Post(Notification notification);

    void Hide();
    
    IBindable<int> UnreadCount { get; }
    
    IEnumerable<Notification> AllNotifications { get; }
}
