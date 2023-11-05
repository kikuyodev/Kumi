using Kumi.Game.Charts;
using Kumi.Game.Gameplay.Difficulty.Skills;

namespace Kumi.Game.Gameplay.Difficulty;

/// <summary>
/// A working difficulty calculator, used in practice to calculate
/// the difficulty of a chart using the foundation of <see cref="DifficultyCalculator"/>
/// </summary>
public class WorkingDifficultyCalculator : DifficultyCalculator
{
    public WorkingDifficultyCalculator(IChart chart) 
        : base(chart)
    {
    }
    public override IEnumerable<Skill> CreateSkills() => new List<Skill>()
    {

    };

    public override float ScaleDifficulty(float difficulty) => difficulty;
}
