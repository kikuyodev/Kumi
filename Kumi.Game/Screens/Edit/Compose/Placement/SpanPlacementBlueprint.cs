using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Pieces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Compose.Placement;

public partial class SpanPlacementBlueprint : PlacementBlueprint
{
    private readonly HitPiece headPiece;
    private readonly HitPiece tailPiece;
    private readonly Container spanPiece;

    private readonly IHasEndTime endTimeNote;

    public SpanPlacementBlueprint(Note note)
        : base(note)
    {
        endTimeNote = (note as IHasEndTime)!;
        RelativeSizeAxes = Axes.None;
        Height = 120;

        InternalChildren = new Drawable[]
        {
            headPiece = new HitPiece
            {
                RelativeSizeAxes = Axes.None,
                Size = new Vector2(72),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft
            },
            spanPiece = new Container
            {
                RelativeSizeAxes = Axes.None,
                Size = new Vector2(54),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                X = 72f / 4f,
                Padding = new MarginPadding { Right = -(72f / 2f) },
                Child = new HitPiece()
            },
            tailPiece = new HitPiece
            {
                RelativeSizeAxes = Axes.None,
                Size = new Vector2(72),
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft
            },
        };
    }

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        => tailPiece.DrawRectangle.Inflate(15f).Contains(tailPiece.ToLocalSpace(screenSpacePos));

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BeginPlacement();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        BeginPlacement(true);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        EndPlacement(true);
        base.OnMouseUp(e);
    }

    private double startTime;
    private Vector2 originalPosition;

    public override void UpdateTimeAndPosition(double? time)
    {
        base.UpdateTimeAndPosition(time);

        if (PlacementActive == PlacementState.Active)
        {
            if ((time ?? EditorClock.CurrentTime) < startTime)
            {
                Note.StartTime = time ?? EditorClock.CurrentTime;
                endTimeNote.EndTime = Math.Abs((time ?? EditorClock.CurrentTime) - startTime) + Note.StartTime;
                headPiece.Position = ToLocalSpace(Composer.Playfield!.ScreenSpacePositionAtTime(time ?? EditorClock.CurrentTime));
                tailPiece.Position = originalPosition;
            }
            else
            {
                Note.StartTime = startTime;
                endTimeNote.EndTime = Math.Abs((time ?? EditorClock.CurrentTime) - startTime) + Note.StartTime;
                tailPiece.Position = ToLocalSpace(Composer.Playfield!.ScreenSpacePositionAtTime(time ?? EditorClock.CurrentTime));
                headPiece.Position = originalPosition;
            }

            spanPiece.X = headPiece.X;
            spanPiece.Width = tailPiece.X - headPiece.X;
        }
        else
        {
            spanPiece.Position = headPiece.Position = tailPiece.Position = ToLocalSpace(Composer.Playfield!.ScreenSpacePositionAtTime(time ?? EditorClock.CurrentTime));

            startTime = Note.StartTime = time ?? EditorClock.CurrentTime;
            originalPosition = spanPiece.Position;
        }
    }
}
