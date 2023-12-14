using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays;

[Cached]
internal interface IOverlayManager
{
    IBindable<OverlayActivation> OverlayActivation { get; }
    
    IDisposable RegisterBlockingOverlay(OverlayContainer container);
    
    void ShowBlockingOverlay(OverlayContainer container);
    void HideBlockingOverlay(OverlayContainer container);
}
