using Kumi.Game.Graphics.Menus;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Graphics.Containers;

public partial class KumiContextMenuContainer : ContextMenuContainer
{
    protected override Menu CreateMenu() => new KumiMenu(Direction.Vertical);
}
