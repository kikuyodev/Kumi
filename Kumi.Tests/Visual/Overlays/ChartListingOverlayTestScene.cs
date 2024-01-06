using Kumi.Game.Overlays;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;

namespace Kumi.Tests.Visual.Overlays;

public partial class ChartListingOverlayTestScene : KumiTestScene
{
    private readonly ListingOverlay overlay;

    public ChartListingOverlayTestScene()
    {
        Add(overlay = new ListingOverlay());
    }
    
    [Test]
    public void TestVisibility()
    {
        AddStep("Hide overlay", () => overlay.Hide());
        AddStep("Show overlay", () => overlay.Show());
        AddAssert("Overlay is visible", () => overlay.State.Value == Visibility.Visible);
    }
}
