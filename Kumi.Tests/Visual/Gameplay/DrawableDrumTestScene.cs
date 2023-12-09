using Kumi.Game.Gameplay.UI;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Input;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;
using osuTK;

namespace Kumi.Tests.Visual.Gameplay;

public partial class DrawableDrumTestScene : KumiTestScene
{
    [Test]
    public void TestDrum()
    {
        AddStep("clear", Clear);
        AddStep("add drum", () =>
        {
            Add(new GameplayKeybindContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new ConstrictedScalingContainer
                {
                    PreferredSize = new Vector2(144),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.8f),
                    Child = new DrawableDrum()
                }
            });
        });
    }
}
