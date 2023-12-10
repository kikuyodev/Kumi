using osu.Framework.Screens;

namespace Kumi.Game.Screens.Edit;

public partial class EditorScreenStack : ScreenStack
{
    public new EditorScreen CurrentScreen => (base.CurrentScreen as EditorScreen)!;
}
