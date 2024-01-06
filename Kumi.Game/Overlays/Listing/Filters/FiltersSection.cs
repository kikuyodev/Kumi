using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Filters;

public partial class FiltersSection : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding
        {
            Left = 8,
            Right = 16,
            Top = 16
        };
        
        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 16),
            Children = new Drawable[]
            {
                new QueryFilter(),
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 12),
                    Children = new Drawable[]
                    {
                        new StatusFilterSelection
                        {
                            Label = "Status"
                        },
                        new StatusFilterSelection
                        {
                            Label = "Genre"
                        }
                    }
                }
            }
        };
    }
}
