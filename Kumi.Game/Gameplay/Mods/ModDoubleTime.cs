using Kumi.Game.Screens.Play;
using osu.Framework.Timing;

namespace Kumi.Game.Gameplay.Mods;

public class ModDoubleTime : Mod
{

    public override string Name { get; } = "Double Time";
    public override string Tag { get; } = "DT";
    public override void Apply(Player player, IAdjustableClock clock)
    {
        clock.Rate = 1.5f;
    }
}
