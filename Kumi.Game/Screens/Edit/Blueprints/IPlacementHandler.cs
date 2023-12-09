using Kumi.Game.Charts.Objects;
using osu.Framework.Allocation;

namespace Kumi.Game.Screens.Edit.Blueprints;

[Cached]
public interface IPlacementHandler
{
    void BeginPlacement(Note note);
    void EndPlacement(Note note, bool commit);
    void Delete(Note note);
}
