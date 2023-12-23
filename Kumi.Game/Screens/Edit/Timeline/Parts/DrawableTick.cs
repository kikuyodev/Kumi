using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class DrawableTick : Circle
{
    public const float TICK_MAX_WIDTH = 3;
    
    public DrawableTick(double startTime)
        : this()
    {
        X = (float) startTime;
    }

    public DrawableTick()
    {
        RelativePositionAxes = Axes.Both;
        RelativeSizeAxes = Axes.Y;

        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        Width = TICK_MAX_WIDTH;
        Height = 0.5f;
    }
}
