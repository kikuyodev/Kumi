using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;
using Kumi.Game.Screens.Edit.Compose.Placement;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class HitCompositionTool : NoteCompositionTool
{
    public HitCompositionTool()
        : base(nameof(DrumHit))
    {
    }

    public override PlacementBlueprint? CreatePlacementBlueprint() => new DrumPlacementBlueprint();
}
