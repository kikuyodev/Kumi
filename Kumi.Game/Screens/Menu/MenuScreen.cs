using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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
    public override bool HideOverlaysOnEnter => true;
    protected override OverlayActivation InitialOverlayActivation => Overlays.OverlayActivation.UserTriggered;

    public override BackgroundScreen CreateBackground() => new MenuBackground();

    private Container logoContainer = null!;
    private Sprite logo = null!;
    private Sprite logoShadow = null!;
    private SpriteText ctaText = null!;

    private Bindable<MenuState> state = new Bindable<MenuState>();

    [Resolved]
    private MusicController musicController { get; set; } = null!;


    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        AddRangeInternal(new Drawable[]
        {
            logoContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
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

    private enum MenuState
    {
        Logo,
        Menu
    }
}
