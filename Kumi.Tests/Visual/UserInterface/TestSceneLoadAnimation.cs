using Kumi.Game.Graphics.Sprites;
using Kumi.Game.Tests;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;

namespace Kumi.Tests.Visual.UserInterface;

public partial class TestSceneLoadAnimation : KumiTestScene
{
    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textures)
    {
        LoadAnimation animation;

        Add(animation = new LoadAnimation
        {
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fit,
            Texture = textures.Get("Logo/logo_animation"),
            Colour = Color4.White
        });

        AddSliderStep("Progress", 0f, 1f, 0f, v => animation.AnimationProgress = v);
    }
}
