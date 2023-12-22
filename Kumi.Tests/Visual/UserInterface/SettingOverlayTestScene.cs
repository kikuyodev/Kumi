using Kumi.Game.Overlays;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Tests.Visual.UserInterface;

public partial class SettingOverlayTestScene : KumiTestScene
{
    private readonly SettingOverlay overlay;
    
    public SettingOverlayTestScene()
    {
        Children = new Drawable[]
        {
            overlay = new SettingOverlay()
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
