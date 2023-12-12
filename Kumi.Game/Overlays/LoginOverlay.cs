using Kumi.Game.Graphics.Containers;
using Kumi.Game.Overlays.Login;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Overlays;

public partial class LoginOverlay : KumiFocusedOverlayContainer
{
    private LoginPanel panel = null!;

    protected override bool StartHidden => true;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Masking = true,
            CornerRadius = 5,
            AutoSizeDuration = 200,
            AutoSizeEasing = Easing.OutQuint,
            Child = panel = new LoginPanel
            {
                RequestHide = Hide
            }
        };
    }

    protected override void PopIn()
    {
        panel.Hidden = false;
        this.FadeInFromZero(200, Easing.OutQuint);

        ScheduleAfterChildren(() => GetContainingInputManager().ChangeFocus(panel));
    }

    protected override void PopOut()
    {
        panel.Hidden = true;
        this.FadeOutFromOne(200, Easing.OutQuint);
    }
}
