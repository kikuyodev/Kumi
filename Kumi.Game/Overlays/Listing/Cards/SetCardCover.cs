using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Overlays.Listing.Cards;

public partial class SetCardCover : Container
{
    private readonly IAPIChartMetadata metadata;

    public SetCardCover(IAPIChartMetadata metadata)
    {
        this.metadata = metadata;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        DelayedLoadWrapper cover;
        
        Children = new Drawable[]
        {
            cover = new DelayedLoadWrapper(new SetCardCoverSprite(metadata)
            {
                RelativeSizeAxes = Axes.Both,
            }, 250),
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientHorizontal(Colours.Gray(0.1f).Opacity(1f), Colours.Gray(0.1f).Opacity(0.9f))
            }
        };
        
        cover.DelayedLoadComplete += d => d.FadeInFromZero(250);
    }
}
