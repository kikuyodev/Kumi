using Kumi.Game.Charts.Objects;
using Kumi.Game.Screens.Edit.Blueprints;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class HitCompositionTool : NoteCompositionTool
{
    public HitCompositionTool()
        : base(nameof(DrumHit))
    {
    }

    public override PlacementBlueprint? CreatePlacementBlueprint() => new DrumPlacementBlueprint();
}
