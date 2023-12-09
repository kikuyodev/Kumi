using Kumi.Game.Input;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace Kumi.Game.Screens;

/// <summary>
/// The central screen stack that handles all screens.
/// </summary>
public partial class KumiScreenStack : ScreenStack, IKeyBindingHandler<GlobalAction>
{
    private bool backButtonEnabled;

    /// <summary>
    /// The background screen stack associated with this screen stack.
    /// </summary>
    [Cached]
    public BackgroundScreenStack BackgroundStack { get; }

    public KumiScreenStack()
    {
        InternalChild = BackgroundStack = new BackgroundScreenStack
        {
            RelativeSizeAxes = Axes.Both,
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre
        };

        ScreenPushed += screenChanged;
        ScreenExited += screenChanged;
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case GlobalAction.Back:
                if (!backButtonEnabled)
                    return false;

                Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    private void screenChanged(IScreen current, IScreen newScreen)
    {
        if (newScreen is KumiScreen newKumiScreen)
            backButtonEnabled = newKumiScreen.AllowBackButton;
    }
}
