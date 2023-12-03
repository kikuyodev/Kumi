namespace Kumi.Game.Charts;

public class BeatDivisorCollection
{
    public BeatDivisorType Type { get; }
    public IReadOnlyList<int> Divisors { get; }

    public BeatDivisorCollection(BeatDivisorType type, IEnumerable<int> divisors)
    {
        Type = type;
        Divisors = divisors.ToArray();
    }
    
    public static readonly BeatDivisorCollection COMMON = new BeatDivisorCollection(BeatDivisorType.Common, new[] { 1, 2, 4, 8, 16 });
    public static readonly BeatDivisorCollection TRIPLETS = new BeatDivisorCollection(BeatDivisorType.Triplets, new[] { 1, 3, 6, 12 });

    public static BeatDivisorCollection Custom(int maxDivisor)
    {
        var divisors = new List<int>();

        for (var i = 1; i < Math.Sqrt(maxDivisor); ++i)
        {
            if (maxDivisor % i != 0)
                continue;
            
            divisors.Add(i);
            divisors.Add(maxDivisor / i);
        }
        
        return new BeatDivisorCollection(BeatDivisorType.Custom, divisors);
    }
}