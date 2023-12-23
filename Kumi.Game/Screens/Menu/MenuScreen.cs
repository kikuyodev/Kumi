using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Online;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Backgrounds;
using Kumi.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Menu;

public partial class MenuScreen : KumiScreen
{
    public override bool HideOverlaysOnEnter => buttons == null || buttons.State == ButtonsState.Hidden;
    protected override OverlayActivation InitialOverlayActivation => Overlays.OverlayActivation.UserTriggered;
    public override PlayerActivity InitialActivity { get; } = new PlayerActivity.Idle();

    public override BackgroundScreen CreateBackground() => new MenuBackground();

    private SelectScreen? selectScreen;

    private Container logoContainer = null!;
    private Sprite logo = null!;
    private Sprite logoShadow = null!;
    private SpriteText ctaText = null!;

    private MenuButtons? buttons;

    [Resolved]
    private MusicController musicController { get; set; } = null!;

    [Resolved]
    private TaskbarOverlay taskbar { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        AddRangeInternal(new Drawable[]
        {
            logoContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Name = "Logo container",
                Children = new Drawable[]
                {
                    logoShadow = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Texture = store.Get("Logo/logo_shadow"),
                        Alpha = 0
                    },
                    logo = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fit,
                        Scale = new Vector2(0.3f),
                        Texture = store.Get("Logo/logo_coloured"),
                        Alpha = 0
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Height = 0.75f,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Child = buttons = new MenuButtons
                {
                    Height = 0.75f,
                    OnPlay = () => this.Push(consumeSelect()),
                    OnEdit = () => this.Push(new EditSelectScreen()),
                    OnExit = this.Exit
                }
            },
            ctaText = new SpriteText
            {
                Shadow = true,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -32,
                Text = "Press any key",
                Font = KumiFonts.GetFont(size: 20)
            },
            new MenuMusicOverlay
            {
                Margin = new MarginPadding
                {
                    Horizontal = 32,
                    Vertical = 40
                }
            },
        });

        buttons.StateChanged += onStateChanged;
        preloadSelect();
    }

    private void preloadSelect()
    {
        if (selectScreen == null)
            LoadComponentAsync(selectScreen = new PlaySelectScreen());
    }

    private Screen consumeSelect()
    {
        var s = selectScreen;
        selectScreen = null;
        return s!;
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        CurrentBackground.Colour = Color4.Black;
        CurrentBackground.FadeColour(Color4.White.Darken(0.5f), 1000, Easing.OutQuint);
        logo.FadeInFromZero(1000, Easing.OutQuint);
        logo.ScaleTo(0.2f, 1000, Easing.OutQuint);
        logoShadow.FadeInFromZero(1000, Easing.OutQuint);

        musicController.EnsurePlayingSong();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);

        musicController.EnsurePlayingSong();
        preloadSelect();
    }

    private const float duration = 500f;

    private void onStateChanged(ButtonsState state)
    {
        switch (state)
        {
            case ButtonsState.Hidden:
                ctaText.FadeIn(duration, Easing.OutQuint);
                ctaText.MoveToY(-32, duration, Easing.OutQuint);
                logoContainer.ResizeHeightTo(1f, duration, Easing.OutQuint);

                OverlayActivation.Value = Overlays.OverlayActivation.UserTriggered;
                taskbar.Hide();
                break;

            case ButtonsState.Visible:
                ctaText.FadeOut(duration, Easing.OutQuint);
                ctaText.MoveToY(0, duration, Easing.OutQuint);
                logoContainer.ResizeHeightTo(0.3f, duration, Easing.OutQuint);

                OverlayActivation.Value = Overlays.OverlayActivation.Any;
                taskbar.Show();
                break;
        }
    }

    private partial class MenuBackground : BackgroundScreen
    {
        [BackgroundDependencyLoader]
        private void load(LargeTextureStore store)
        {
            Background background;
            SetBackgroundImmediately(background = new Background());

            background.SetBackground(store.Get("Backgrounds/bg1"));
        }
    }
}
