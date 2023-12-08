using Kumi.Game.Online.API.Accounts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace Kumi.Game.Graphics.Sprites;

public partial class AvatarSprite : Sprite
{
    private readonly IAccount? user;

    public AvatarSprite(IAccount? user)
    {
        this.user = user;

        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fit;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        Texture = store.Get($"http://localhost:3333/cdn/avatars/{user?.Id ?? 0}");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(200, Easing.OutQuint);
    }
}
