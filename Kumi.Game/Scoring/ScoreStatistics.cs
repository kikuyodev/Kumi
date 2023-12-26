using Realms;

namespace Kumi.Game.Scoring;

public class ScoreStatistics : EmbeddedObject
{
    public int Miss { get; set; }
    public int Good { get; set; }
    public int Ok { get; set; }
    public int Bad { get; set; }
    
    public ScoreStatistics DeepClone()
    {
        return new ScoreStatistics
        {
            Miss = Miss,
            Good = Good,
            Ok = Ok,
            Bad = Bad
        };
    }
}
