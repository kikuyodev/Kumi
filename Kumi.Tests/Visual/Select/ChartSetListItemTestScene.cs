using Kumi.Game.Screens.Select.List;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Tests.Visual.Select;

public partial class ChartSetListItemTestScene : KumiTestScene
{
    private readonly BindableBool selected = new BindableBool();
    
    [Test]
    public void TestSimple()
    {
        AddStep("clear", Clear);
        AddStep("add item", () =>
        {
            Add(new ChartSetListItem(TestResources.CreateChartSet())
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 0.75f,
                Selected =
                {
                    BindTarget = selected
                }
            });
        });
        
        AddToggleStep("toggle selected", v => selected.Value = v);
    }
}
