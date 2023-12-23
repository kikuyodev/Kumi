using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Placement;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class DrumCompositionTool : NoteCompositionTool
{
    public DrumCompositionTool()
        : base("Drum")
    {
    }

    public override PlacementBlueprint? CreatePlacementBlueprint()
        => new DrumPlacementBlueprint();
}
