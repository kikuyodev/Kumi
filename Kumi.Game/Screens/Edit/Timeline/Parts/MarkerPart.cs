using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class MarkerPart : TimelinePart
{
    private MarkerVisualisation marker = null!;

    [Resolved]
    private EditorClock clock { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(marker = new MarkerVisualisation());
    }

    protected override bool OnDragStart(DragStartEvent e) => true;

    protected override void OnDrag(DragEvent e)
    {
        seekToPosition(e.ScreenSpaceMousePosition);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        seekToPosition(e.ScreenSpaceMousePosition);
        return base.OnMouseDown(e);
    }

    private ScheduledDelegate? scheduledSeek;
    
    private void seekToPosition(Vector2 screenPosition)
    {
        scheduledSeek?.Cancel();
        scheduledSeek = Schedule(() =>
        {
            var markerPos = Math.Clamp(ToLocalSpace(screenPosition).X, 0, DrawWidth);
            clock.Seek(markerPos / DrawWidth * clock.TrackLength);
        });
    }

    protected override void Update()
    {
        base.Update();
        marker.X = (float) clock.CurrentTime;
    }

    protected override void LoadChart(IChart chart)
    {
    }

    private partial class MarkerVisualisation : CompositeDrawable
    {
        public MarkerVisualisation()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;
            RelativePositionAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;
            Colour = Colours.RED_ACCENT;

            InternalChildren = new Drawable[]
            {
                new Triangle
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    Scale = new Vector2(1, -1),
                    Size = new Vector2(10, 5)
                },
                new Triangle
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = new Vector2(10, 5)
                },
                new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    Width = 1f,
                    EdgeSmoothness = new Vector2(1, 0)
                }
            };
        }
    }
}
