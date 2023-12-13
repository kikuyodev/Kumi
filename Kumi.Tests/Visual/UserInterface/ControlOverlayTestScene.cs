using Kumi.Game.Overlays;
using Kumi.Game.Overlays.Control;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Tests.Visual.UserInterface;

public partial class ControlOverlayTestScene : KumiTestScene
{
    private ControlOverlay overlay = null!;

    [SetUp]
    public void Setup()
    {
        Children = new Drawable[]
        {
            overlay = new ControlOverlay()
        };
    }

    [Test]
    public void TestVisibility()
    {
        AddStep("Hide overlay", () => overlay.Hide());

        AddStep("Show overlay", () => overlay.Show());
        AddAssert("Overlay is visible", () => overlay.State.Value == Visibility.Visible);

        AddStep("Send notification", () => overlay.Post(new BasicNotification
        {
            Header = "Test",
            Message = "This is a test notification."
        }));
    }
}
