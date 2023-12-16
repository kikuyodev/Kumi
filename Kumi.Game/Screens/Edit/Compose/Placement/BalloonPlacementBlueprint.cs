using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Pieces;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Compose.Placement;

public partial class BalloonPlacementBlueprint : PlacementBlueprint
{
    public new Balloon Note => (Balloon) base.Note;
    
    private readonly HitPiece piece;

    public BalloonPlacementBlueprint()
        : base(new Balloon())
    {
        RelativeSizeAxes = Axes.None;
        Height = 120;
        InternalChild = piece = new HitPiece
        {
            RelativeSizeAxes = Axes.None,
            Size = new Vector2(72),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        BeginPlacement();
    }

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => piece.ReceivePositionalInputAt(screenSpacePos);

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
    
    public override void UpdateTimeAndPosition(double? time)
    {
        base.UpdateTimeAndPosition(time);

        if (PlacementActive == PlacementState.Active)
        {
            if ((time ?? EditorClock.CurrentTime) < startTime)
            {
                Note.StartTime = time ?? EditorClock.CurrentTime;
                Note.EndTime = startTime;
            }
            else
            {
                Note.StartTime = startTime;
                Note.EndTime = time ?? EditorClock.CurrentTime;
            }
        }
        else
        {
            startTime = Note.StartTime = time ?? EditorClock.CurrentTime;
        }
        
        piece.Position = ToLocalSpace(Composer.Playfield!.ScreenSpacePositionAtTime(time ?? EditorClock.CurrentTime));
        piece.X -= piece.DrawWidth / 2;
    }
}
