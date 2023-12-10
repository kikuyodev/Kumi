using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Edit;

public abstract partial class EditorScreen : Screen
{
    [Resolved]
    protected EditorChart EditorChart { get; private set; } = null!;
    
    private readonly Container content;

    public readonly EditorScreenMode Type;

    protected EditorScreen(EditorScreenMode type)
    {
        Type = type;

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = content = new PopoverContainer { RelativeSizeAxes = Axes.Both };
    }
    
    public readonly BindableBool CanCopy = new BindableBool();

    public virtual void Copy(bool cut)
    {
    }
    
    public readonly BindableBool CanPaste = new BindableBool();

    public virtual void Paste()
    {
    }
}
