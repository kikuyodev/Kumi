using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Screens.Edit.Menus;

public partial class EditorScreenTabControl : KumiTabControl<EditorScreenMode>
{
    protected override TabItem<EditorScreenMode> CreateTabItem(EditorScreenMode value) => new EditorScreenTabItem(value);

    public EditorScreenTabControl()
    {
        Enum.GetValues<EditorScreenMode>().ForEach(AddItem);
    }
    
    private partial class EditorScreenTabItem : KumiTabItem
    {
        public EditorScreenTabItem(EditorScreenMode value)
            : base(value)
        {
            var arr = Enum.GetValues<EditorScreenMode>();
            Width = 1f / arr.Length;
        }
    }
}

public enum EditorScreenMode
{
    Setup,
    Compose,
    Timing,
    Verify
}