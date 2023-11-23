namespace Kumi.Game.Gameplay.Difficulty.Skills;

/// <summary>
/// A basic representation of a skill that a player can have.
/// </summary>
public abstract class Skill
{
    /// <summary>
    /// The base rate at which strain peaks decays in the final difficulty value.
    /// </summary>
    protected virtual float StrainDecayRate => 1.1f;

    /// <summary>
    /// The base multiplier for strain decay.
    /// </summary>
    protected virtual float StrainDecayBase => 1.1f;

    /// <summary>
    /// The weight of which strain peaks decay.
    /// </summary>
    protected virtual float StrainDecayWeight => 0.95f;

    /// <summary>
    /// The length of each section of a map to generate strain peaks for.
    /// </summary>
    protected virtual int SectionLength => 500;

    private List<float> strainPeaks { get; } = new List<float>();
    private float currentSectionPeak { get; set; }
    private int currentSectionEnd { get; set; }

    public SkillResult Calculate()
    {
        float finalDifficulty = 0;
        var weight = StrainDecayRate;

        foreach (var peak in strainPeaks.Where(p => p > 0).OrderDescending())
        {
            finalDifficulty += peak * weight;
            weight *= StrainDecayWeight;
        }

        return new SkillResult
        {
            DifficultyValue = finalDifficulty,
            Skill = this
        };
    }

    public void ProcessStrain(CalculableNote current)
    {
        if (current.Index == 0)
            currentSectionEnd = (int) current.StartTime + SectionLength;

        while (currentSectionEnd < current.StartTime)
        {
            strainPeaks.Add(currentSectionPeak);
            currentSectionPeak = initialStrain(current);
            currentSectionEnd += SectionLength;
        }

        currentSectionPeak = Math.Max(currentSectionPeak, StrainValueFor(current));
    }

    protected abstract float StrainValueFor(CalculableNote current);

    private float initialStrain(CalculableNote current)
    {
        var time = current.StartTime;

        if (current.Index >= 0)
            time -= current.Previous(1)?.StartTime ?? 0.0f;

        return currentSectionPeak * (float) Math.Pow(StrainDecayBase, (currentSectionEnd - time) / 1000.0f);
    }
}
