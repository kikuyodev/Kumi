using Kumi.Game.Screens.Select.List;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;

namespace Kumi.Tests.Visual.Select;

public partial class ListSelectTestScene : KumiTestScene
{
    [Test]
    public void TestSimple()
    {
        AddStep("clear", () => Clear(true));
        AddStep("add item", () =>
        {
            Add(new ListSelect
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 0.75f
            });
        });
    }
}
