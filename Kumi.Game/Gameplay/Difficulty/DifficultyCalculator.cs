using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Difficulty.Objects;
using Kumi.Game.Gameplay.Difficulty.Skills;

namespace Kumi.Game.Gameplay.Difficulty;

/// <summary>
/// A class that calculates the final difficulty of a chart.
///
/// This class utilizes a strain-based strategy to calculate the difficulty of a chart,
/// which involves perceiving the difficulty of a chart by obtaining a set of strain values
/// based on the chart's objects and then calculating the final difficulty based on the
/// strain values.
///
/// The strain values are calculated by using a set of strain calculators, which are
/// referred to as skills. Each skill calculates the strain values of a specific aspect
/// of a chart, such as tapping, streaming, or aiming. The strain values of each skill
/// are then combined together to obtain the final strain values of the chart.
///
/// Each skill then returns a final difficulty result, which acts as a final value to
/// be added to the final difficulty of the chart.
///
/// This class is abstract, as it does not implement any skills. Instead, it is up to
/// <see cref="WorkingDifficultyCalculator"/> to implement the skills, and then the
/// final result is returned by <see cref="Calculate"/>.
/// </summary>
public abstract class DifficultyCalculator
{
    /// <summary>
    /// The difficulty multiplier of the calculator for final difficulty scaling.
    /// </summary>
    protected virtual float DifficultyMultiplier => 1.0f;
    
    /// <summary>
    /// The chart currently being processed.
    /// </summary>
    public IChart CurrentChart { get; }

    protected DifficultyCalculator(IChart chart)
    {
        // TODO: Use WorkingChart when we can make a playable chart from a chart.
        CurrentChart = chart;
    }

    /// <summary>
    /// Calculates the final difficulty of the chart.
    /// </summary>
    public DifficultyResult Calculate() => createFinalResult();
    
    /// <summary>
    /// Creates the skills to be used by the difficulty calculator.
    /// </summary>
    public abstract IEnumerable<Skill> CreateSkills();
    
    /// <summary>
    /// Scales the final difficulty value based on the difficulty of the chart.
    /// </summary>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    public abstract float ScaleDifficulty(float difficulty);
    
    private DifficultyResult createFinalResult()
    {
        if (CurrentChart.Notes.Count == 0)
            return createEmptyResult();
        
        var skillResults = new List<SkillResult>();

        foreach (var skill in CreateSkills())
        {
            foreach (var cNote in createCalculableNotes())
            {
                skill.ProcessStrain(cNote);
            }
            
            var result = skill.Calculate();
            skillResults.Add(result);
        }

        return new DifficultyResult()
        {
            DifficultyRating = ScaleDifficulty(skillResults.Sum(r =>
            {
                if (r.DifficultyValue < 0.0f)
                    return 0.0f;
                
                return r.DifficultyValue * DifficultyMultiplier;
            }))
        };
    }
    
    private IEnumerable<CalculableNote> createCalculableNotes()
    {
        var notes = new List<CalculableNote>();
        
        foreach (var note in CurrentChart.Notes)
        {
            switch (note.Type)
            {
                // TODO: Implement other note types.
                case NoteType.Kat:
                case NoteType.Don:
                    notes.Add(new CalculableDrumNote(note, notes, notes.Count));
                    break;

                default:
                    notes.Add(new CalculableNote(note, notes, notes.Count));
                    break;
            }
        }

        return notes;
    }

    private DifficultyResult createEmptyResult() => new DifficultyResult()
    {
        DifficultyRating = 0.0f
    };
}
