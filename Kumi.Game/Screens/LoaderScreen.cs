using Kumi.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK;

namespace Kumi.Game.Screens;

public partial class LoaderScreen : KumiScreen
{
    private LoadAnimation loadAnimation = null!;
    private Sprite logo = null!;

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore store)
    {
        AddRangeInternal(new Drawable[]
        {
            logo = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fit,
                Scale = new Vector2(0.3f),
                Texture = store.Get("Logo/logo"),
                Colour = new Colour4(1f, 1f, 1f, 0.1f)
            },
            loadAnimation = new LoadAnimation
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fit,
                Scale = new Vector2(0.3f),
                Texture = store.Get("Logo/logo_animation"),
                Colour = new Colour4(1f, 1f, 1f, 0.1f)
            }
        });
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        logo.FadeOut(1000, Easing.OutQuint);
        logo.ScaleTo(0.2f, 1000, Easing.OutQuint);
        loadAnimation.FadeOut(1000, Easing.OutQuint);
        loadAnimation.ScaleTo(0.2f, 1000, Easing.OutQuint);

        return base.OnExiting(e);
    }

    public void SetProgress(float progress)
    {
        loadAnimation.TransformTo(nameof(loadAnimation.AnimationProgress), progress, 500, Easing.OutQuint);
    }
}
