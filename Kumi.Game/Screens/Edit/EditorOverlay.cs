using Kumi.Game.Charts;
using Kumi.Game.Screens.Edit.Timeline;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit;

[Cached]
public partial class EditorOverlay : Container
{
    public IBindable<WorkingChart> Chart { get; } = new Bindable<WorkingChart>();

    public EditorOverlay()
    {
        Chart.BindValueChanged(_ => constructDisplay(), true);
    }
    
    private void constructDisplay()
    {
        Padding = new MarginPadding(12);

        Children = new Drawable[]
        {
            new TimelineBar
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            }
        };
    }
}
