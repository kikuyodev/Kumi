using Kumi.Game.Input;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Menu;

public partial class MenuButtons : Container, IStateful<ButtonsState>, IKeyBindingHandler<GlobalAction>
{
    private ButtonsState state = ButtonsState.Hidden;

    public event Action<ButtonsState>? StateChanged;

    public Action? OnPlay;
    public Action? OnEdit;
    public Action? OnSettings;
    public Action? OnExit;

    public ButtonsState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;
            updateState();
            StateChanged?.Invoke(State);
        }
    }

    [Resolved]
    private KumiGame? game { get; set; }

    public MenuButtons()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    private FillFlowContainer<MenuButton> buttons = null!;

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textures, GameHost host)
    {
        Child = buttons = new FillFlowContainer<MenuButton>
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(10),
        };

        buttons.Add(new MenuButton("Play", Colour4.FromHex("3377FF"), textures.Get("Backgrounds/bg1"), () => OnPlay?.Invoke()));
        buttons.Add(new MenuButton("Edit", Colour4.FromHex("FFB133"), textures.Get("Backgrounds/bg1"), () => OnEdit?.Invoke()));
        if (host.CanExit)
            buttons.Add(new MenuButton("Quit", Colour4.FromHex("FF3366"), textures.Get("Backgrounds/bg1"), () => OnExit?.Invoke()));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        // Hide by default
        updateState();
        FinishTransforms(true);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat || e.ControlPressed || e.ShiftPressed || e.AltPressed || e.SuperPressed)
            return false;

        // Hacky way to exit the game if the user presses escape
        if (State == ButtonsState.Hidden && e.Key != Key.Escape)
        {
            State = ButtonsState.Visible;
            return true;
        }

        return base.OnKeyDown(e);
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case GlobalAction.Back:
                return hideState();

            case GlobalAction.Select:
                if (State == ButtonsState.Visible)
                {
                    OnPlay?.Invoke();
                    return true;
                }

                return false;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    private void updateState()
    {
        switch (State)
        {
            case ButtonsState.Hidden:
                buttons.FadeOut(200);
                break;

            case ButtonsState.Visible:
                buttons.FadeIn(200);
                break;
        }
    }

    private bool hideState()
    {
        if (State == ButtonsState.Visible)
        {
            State = ButtonsState.Hidden;
            return true;
        }

        return false;
    }
}

public enum ButtonsState
{
    Hidden,
    Visible
}
