using osu.Framework.Allocation;
using osuTK;

namespace Kumi.Game.Screens.Edit;

[Cached]
public interface ISnapProvider
{
    double TimeAtScreenSpacePosition(Vector2 screenSpacePosition);

    double SnapTime(double time, int beatDivisor);
}
