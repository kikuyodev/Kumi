using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Pieces;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public partial class DrumPlacementBlueprint : PlacementBlueprint
{
    private readonly HitPiece piece;

    [Resolved]
    private ComposeScreen composer { get; set; } = null!;

    [Resolved]
    private EditorClock clock { get; set; } = null!;

    public DrumPlacementBlueprint()
        : base(new DrumHit())
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

        EndPlacement(true);
        return true;
    }

    public override void UpdateTimeAndPosition(double? time)
    {
        piece.Position = ToLocalSpace(composer.Playfield!.ScreenSpacePositionAtTime(time ?? clock.CurrentTime));
        piece.X -= piece.DrawWidth / 2;
        base.UpdateTimeAndPosition(time);
    }
}
