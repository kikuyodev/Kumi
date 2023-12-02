using Kumi.Game.Charts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Edit;

public abstract partial class EditorScreen : Screen
{
    [Resolved]
    protected Chart Chart { get; private set; } = null!;
    
    private readonly Container content;

    public readonly EditorScreenMode Type;

    protected EditorScreen(EditorScreenMode type)
    {
        Type = type;

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = content = new PopoverContainer { RelativeSizeAxes = Axes.Both };
    }
}
