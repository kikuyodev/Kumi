using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Placement;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class BigDrumCompositionTool : NoteCompositionTool
{
    public BigDrumCompositionTool()
        : base("Big Drum")
    {
    }

    public override PlacementBlueprint? CreatePlacementBlueprint()
        => new BigDrumPlacementBlueprint();
}
