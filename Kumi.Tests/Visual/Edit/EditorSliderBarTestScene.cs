using Kumi.Game.Screens.Edit.Components;
using Kumi.Game.Tests;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;

namespace Kumi.Tests.Visual.Edit;

public partial class EditorSliderBarTestScene : KumiTestScene
{
    public EditorSliderBarTestScene()
    {
        Add(new EditorSliderBar<double>
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Width = 0.5f,
            BarHeight = 24,
            BarColour = Color4Extensions.FromHex("0080FF"),
            BackgroundColour = Color4Extensions.FromHex("1A1A1A"),
            Current = new BindableDouble
            {
                MaxValue = 1d,
                MinValue = 0d,
                Value = 0.5d
            }
        });
    }
}
