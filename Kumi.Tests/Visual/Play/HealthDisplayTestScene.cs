using Kumi.Game.Screens.Play.HUD;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;

namespace Kumi.Tests.Visual.Play;

public partial class HealthDisplayTestScene : KumiTestScene
{
    private HealthDisplay display = null!;

    [Test]
    public void TestDisplay()
    {
        display = new HealthDisplay
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Scale = new Vector2(2f)
        };

        AddStep("clear", Clear);
        AddStep("add display", () => Add(display));
        AddSliderStep("set health", 0, 1, 0.5, v => display.Current.Value = v);
    }
}
