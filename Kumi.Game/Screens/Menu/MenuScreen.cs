using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Screens.Backgrounds;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Menu;

public partial class MenuScreen : KumiScreen
{
    private Sprite logo = null!;
    private Sprite logoShadow = null!;
    
    public override BackgroundScreen CreateBackground() => new MenuBackground();

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        AddRangeInternal(new Drawable[]
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
