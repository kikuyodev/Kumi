using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Edit.Compose.Pieces;

public partial class HitPiece : CircularContainer
{
    public HitPiece()
    {
        RelativeSizeAxes = Axes.Both;
        Masking = true;
        BorderThickness = 3;
        BorderColour = Color4.Yellow;
        Colour = Color4.Yellow;
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0.25f
        };
    }
}
