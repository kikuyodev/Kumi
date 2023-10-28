using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game;

public partial class KumiGame : KumiGameBase
{
    private ScreenStack screenStack = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        // push...
    }
}
