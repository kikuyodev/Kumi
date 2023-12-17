using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays.Chat;

public partial class ChannelScrollContainer : BasicScrollContainer
{
    private bool trackingNewContent = true;

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (trackingNewContent && !IsScrolledToEnd())
            ScrollToEnd();
    }

    private void updateTrackState()
        => trackingNewContent = IsScrolledToEnd(10);

    protected override void OnUserScroll(float value, bool animated = true, double? distanceDecay = null)
    {
        base.OnUserScroll(value, animated, distanceDecay);
        updateTrackState();
    }

    public new void ScrollToStart(bool animated = true, bool allowDuringDrag = false)
    {
        base.ScrollToStart(animated, allowDuringDrag);
        updateTrackState();
    }

    public new void ScrollToEnd(bool animated = true, bool allowDuringDrag = false)
    {
        base.ScrollToEnd(animated, allowDuringDrag);
        updateTrackState();
    }
}
