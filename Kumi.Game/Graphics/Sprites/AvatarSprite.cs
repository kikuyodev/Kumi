using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Accounts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Graphics.Sprites;

public partial class AvatarSprite : Sprite
{
    private readonly IAccount? user;

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;

    public AvatarSprite(IAccount? user)
    {
        this.user = user;

        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fit;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store, IRenderer renderer)
    {
        Texture = store.Get($"{api.EndpointConfiguration.WebsiteUri}/cdn/avatars/{user?.Id ?? 0}");

        if (Texture == null)
        {
            Texture = renderer.WhitePixel;
            Colour = Colour4.Gray;
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(200, Easing.OutQuint);
    }
}
