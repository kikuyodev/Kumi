using Kumi.Game.Screens.Play;
using osu.Framework.Timing;

namespace Kumi.Game.Gameplay.Mods;

/// <summary>
/// An abstract class that represents a modification to the gameplay.
/// </summary>
public abstract class Mod
{
    /// <summary>
    /// The name of the mod.
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// The named tag of the mod, usually an abbreviation of the name.
    /// </summary>
    public abstract string Tag { get; }

    /// <summary>
    /// A description of the mod, used to explain what the mod does.
    /// </summary>
    public virtual string Description => "No description provided.";
    
    /// <summary>
    /// A list of mods that are incompatible with this mod.
    /// </summary>
    public virtual Type[] IncompatibleMods => new Type[0];
    
    /// <summary>
    /// Applies the mod to the player.
    /// </summary>
    public abstract void Apply(Player player, IAdjustableClock clock);
}
