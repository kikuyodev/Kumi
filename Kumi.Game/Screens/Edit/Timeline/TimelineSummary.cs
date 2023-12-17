using Kumi.Game.Screens.Edit.Timeline.Parts;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineSummary : Container
{
    public TimelineSummary()
    {
        Children = new Drawable[]
        {
            new MarkerPart { RelativeSizeAxes = Axes.Both },
            new TimingPointsPart { RelativeSizeAxes = Axes.Both },
        };
    }
}
