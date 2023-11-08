namespace Kumi.Game.Gameplay.Difficulty.Skills;

/// <summary>
/// A result of a skill calculation.
/// </summary>
public class SkillResult
{
    /// <summary>
    /// The final difficulty value of the skill.
    /// </summary>
    public float DifficultyValue { get; set; }

    /// <summary>
    /// The skill that calculated the result.
    /// </summary>
    public Skill Skill { get; set; } = null!;
}
