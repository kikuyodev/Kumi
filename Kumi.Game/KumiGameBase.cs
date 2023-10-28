using Kumi.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;

namespace Kumi.Game;

public partial class KumiGameBase : osu.Framework.Game
{
    protected override Container<Drawable> Content { get; }
    
    protected KumiGameBase()
    {
        base.Content.Add(Content = new DrawSizePreservingFillContainer
        {
            TargetDrawSize = new Vector2(1920, 1080)
        });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Resources.AddStore(new DllResourceStore(KumiResources.Assembly));
    }
}
