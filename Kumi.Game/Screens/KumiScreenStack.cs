using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game.Screens;

/// <summary>
/// The central screen stack that handles all screens.
/// </summary>
public partial class KumiScreenStack : ScreenStack
{
    /// <summary>
    /// The background screen stack associated with this screen stack.
    /// </summary>
    public BackgroundScreenStack BackgroundStack { get; set; }

    public KumiScreenStack()
    {
        InternalChild = BackgroundStack = new BackgroundScreenStack
        {
            RelativeSizeAxes = Axes.Both,
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre
        };
    }
}
