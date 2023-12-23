using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class TimingPointPiece : CompositeDrawable
{
    private readonly TimingPoint point;
    private readonly bool isInherited;
    
    private SpriteText label = null!;

    public TimingPointPiece(TimingPoint point)
    {
        this.point = point;
        isInherited = point.PointType == TimingPointType.Inherited;

        AutoSizeAxes = Axes.X;
        Height = 24;
        Margin = new MarginPadding { Bottom = 4 };

        Origin = Anchor.BottomCentre;
        Anchor = Anchor.BottomCentre;    
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Circle
            {
                RelativeSizeAxes = Axes.Y,
                Width = 3,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Colours.YELLOW_ACCENT_LIGHT
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colours.YELLOW_ACCENT_LIGHT
                    },
                    label = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Padding = new MarginPadding { Vertical = 1, Horizontal = 4 },
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 12),
                        Colour = Colours.Gray(0.05f),
                        Text = isInherited ? $"{point.RelativeScrollSpeed:N2}x" : $"{((UninheritedTimingPoint)point).BPM:N1} BPM"
                    }
                }
            }
        };

        if (!isInherited)
        {
            var uninheritedPoint = (UninheritedTimingPoint) point;
            uninheritedPoint.GetBindableBPM().BindValueChanged(v =>
            {
                label.Text = $"{v.NewValue:N1} BPM";
            });
        }
    }
}
