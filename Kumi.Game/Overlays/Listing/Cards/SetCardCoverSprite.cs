using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Overlays.Listing.Cards;

public partial class SetCardCoverSprite : BufferedContainer
{
    private readonly IAPIChartMetadata metadata;

    public SetCardCoverSprite(IAPIChartMetadata metadata)
        : base(cachedFrameBuffer: true)
    {
        this.metadata = metadata;

        RedrawOnScale = false;
    }

    [BackgroundDependencyLoader]
    private void load(IAPIConnectionProvider api, LargeTextureStore store)
    {
        Child = new Sprite
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill,
            Texture = store.Get($"{api.EndpointConfiguration.WebsiteUri}/cdn/backgrounds/{metadata.Id}")
        };
    }
}
