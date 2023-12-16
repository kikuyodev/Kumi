using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Placement;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class BalloonCompositionTool : NoteCompositionTool
{
    public BalloonCompositionTool()
        : base(nameof(Balloon))
    {
    }

    public override PlacementBlueprint? CreatePlacementBlueprint()
        => new BalloonPlacementBlueprint();
}
