using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Timeline;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class TimingScreen : EditorScreenWithTimeline
{
    public TimingScreen()
        : base(EditorScreenMode.Timing)
    {
    }

    protected override Drawable CreateMainContent()
    {
        return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding
            {
                Top = 12,
                Bottom = 12 + BottomBarTimeline.HEIGHT + 12,
                Horizontal = 12,
            },
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 5,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.Gray(0.05f)
                        }
                    }
                }
            }
        };
    }
}
