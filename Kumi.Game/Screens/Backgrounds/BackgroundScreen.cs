using Kumi.Game.Graphics.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Backgrounds;

public partial class BackgroundScreen : Screen
{
    /// <summary>
    /// The stack of backgrounds that this screen will display.
    /// </summary>
    public BackgroundStack BackgroundStack { get; }
    
    public BackgroundScreen()
    {
        InternalChild = BackgroundStack = new BackgroundStack()
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };
    }
    
    public void SetBackgroundImmediately(Background background) => BackgroundStack.SetBackgroundImmediately(background);
}
