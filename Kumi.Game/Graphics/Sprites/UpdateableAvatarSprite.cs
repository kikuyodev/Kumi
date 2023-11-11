using Kumi.Game.Online.API.Accounts;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Graphics.Sprites;

public partial class UpdateableAvatarSprite : ModelBackedDrawable<APIAccount?>
{
    public APIAccount? Account
    {
        get => Model;
        set => Model = value;
    }

    public UpdateableAvatarSprite(APIAccount? account = null)
    {
        Account = account;
        Masking = true;
        CornerRadius = 5;
    }

    protected override Drawable CreateDrawable(APIAccount? account)
        => new AvatarSprite(account);
}
