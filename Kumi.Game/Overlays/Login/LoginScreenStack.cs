using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game.Overlays.Login;

public partial class LoginScreenStack : ScreenStack
{
    public new Axes AutoSizeAxes
    {
        get => base.AutoSizeAxes;
        set => base.AutoSizeAxes = value;
    }
    
    public new MarginPadding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }
}
