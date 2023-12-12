using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays;

[Cached]
internal interface IOverlayManager
{
    IDisposable RegisterBlockingOverlay(OverlayContainer container);
    
    void ShowBlockingOverlay(OverlayContainer container);
    void HideBlockingOverlay(OverlayContainer container);
}
