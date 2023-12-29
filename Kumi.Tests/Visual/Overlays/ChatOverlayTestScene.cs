using Kumi.Game.Overlays;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Tests.Visual.Overlays;

public partial class ChatOverlayTestScene : KumiTestScene
{
    private ChatOverlay overlay = null!;
    
    [SetUp]
    public void Setup()
    {
        Children = new Drawable[]
        {
            overlay = new ChatOverlay()
        };
    }
    
    [Test]
    public void TestVisibility()
    {
        AddStep("Hide overlay", () => overlay.Hide());

        AddStep("Show overlay", () => overlay.Show());
        AddAssert("Overlay is visible", () => overlay.State.Value == Visibility.Visible);
    }
}
