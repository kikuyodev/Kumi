using Kumi.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Overlays.Listing;

public partial class ChartCoverSprite : BufferedContainer
{
    private readonly IAPIModal modal;

    public ChartCoverSprite(IAPIModal modal)
        : base(cachedFrameBuffer: true)
    {
        this.modal = modal;

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
            Texture = store.Get($"{api.EndpointConfiguration.WebsiteUri}/cdn/backgrounds/{modal.Id}")
        };
    }
}
