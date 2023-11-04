using Kumi.Game.Screens;
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
        /*AddRange(new Drawable[]
        {
            screenStack = ScreenStack
        });*/
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        // push...
    }
}
