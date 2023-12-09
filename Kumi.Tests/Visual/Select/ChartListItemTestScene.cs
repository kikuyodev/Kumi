using Kumi.Game.Screens.Select.List;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Tests.Visual.Select;

public partial class ChartListItemTestScene : KumiTestScene
{
    private readonly BindableBool selected = new BindableBool();
    
    [Test]
    public void TestSimple()
    {
        AddStep("clear", Clear);
        AddStep("add item", () =>
        {
            Add(new ChartListItem(TestResources.CreateChartSet().Charts[0])
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Selected =
                {
                    BindTarget = selected
                }
            });
        });
        
        AddToggleStep("toggle selected", v => selected.Value = v);
    }
}
