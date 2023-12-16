using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Overlays;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Tests.Visual.UserInterface;

public partial class AccountRegistrationOverlayTestScene : KumiTestScene
{
    private readonly AccountRegistrationOverlay overlay;

    private IBindable<APIAccount> localAccount = null!;
    
    public AccountRegistrationOverlayTestScene()
    {
        Children = new Drawable[]
        {
            overlay = new AccountRegistrationOverlay()
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        localAccount = API.LocalAccount.GetBoundCopy();
    }

    [Test]
    public void TestVisibility()
    {
        AddStep("Hide overlay", () => overlay.Hide());
        AddStep("Log out", () => API.Logout());
        
        AddStep("Show overlay", () => overlay.Show());
        AddAssert("Overlay is visible", () => overlay.State.Value == Visibility.Visible);
    }
}
