using Kumi.Game;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace Kumi.Tests;

public partial class KumiTestBrowser : KumiGameBase
{
    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        AddRange(new Drawable[]
        {
            new TestBrowser("Kumi.Tests"),
            new CursorContainer()
        });
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        host.Window.CursorState |= CursorState.Hidden;
    }
}
