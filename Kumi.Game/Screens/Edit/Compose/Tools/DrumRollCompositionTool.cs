using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Placement;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class DrumRollCompositionTool : NoteCompositionTool
{
    public DrumRollCompositionTool()
        : base(nameof(DrumRoll))
    {
    }

    public override PlacementBlueprint? CreatePlacementBlueprint()
        => new DrumRollPlacementBlueprint();
}
