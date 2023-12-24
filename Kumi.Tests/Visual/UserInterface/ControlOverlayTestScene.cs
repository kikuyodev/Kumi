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
    private TestProgressNotification progressNotification = null!;

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
        
        AddStep("Send progress notification", () => overlay.Post(progressNotification =new TestProgressNotification(100)
        {
            Header = "Test",
            Message = "This is a test notification."
        }));
        AddStep("Send cancellable progress notification", () => overlay.Post(new TestProgressNotification(100)
        {
            Header = "Test",
            Message = "This is a test notification.",
            Cancellable = true
        }));
        
        AddStep("Set progress to 50", () => progressNotification.Current = 50);
        AddStep("Set progress to 100", () => progressNotification.Current = 100);
        AddUntilStep("Wait for progress notification to close", () => progressNotification.IsClosed);
    }
    
    internal partial class TestProgressNotification : ProgressNotification
    {
        public TestProgressNotification(int target)
            : base(target)
        {
        }
    }
}
