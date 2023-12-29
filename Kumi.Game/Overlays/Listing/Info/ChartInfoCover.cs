using Kumi.Game.Graphics;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class ChartInfoCover : CompositeDrawable
{
    private int depth;
    private ChartCoverSprite? sprite;

    private Container coverContainer = null!;
    
    [BackgroundDependencyLoader]
    private void load(Bindable<APIChartSet?> currentChartSet)
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.07f)
            },
            coverContainer = new Container
            {
                RelativeSizeAxes = Axes.Both
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.07f),
                Alpha = 0.75f
            }
        };
        
        currentChartSet.BindValueChanged(v => pushBackground(v.NewValue), true);
    }

    private void pushBackground(APIChartSet? chartSet)
    {
        if (chartSet == null)
            return;
        
        if (sprite == null)
        {
            sprite = createSprite(chartSet);
            coverContainer.Add(sprite);
            return;
        }

        var newSprite = createSprite(chartSet);
        coverContainer.Add(newSprite);
        
        sprite?.FadeOut(200).OnComplete(_ =>
        {
            coverContainer.Remove(sprite, true);
            sprite = newSprite;
        });
    }

    private ChartCoverSprite createSprite(IAPIModal modal)
        => new ChartCoverSprite(modal)
        {
            Depth = depth--,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both
        };
}
