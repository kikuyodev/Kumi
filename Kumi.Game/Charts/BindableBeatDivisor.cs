using osu.Framework.Bindables;

namespace Kumi.Game.Charts;

public class BindableBeatDivisor : BindableInt
{
    public static readonly int[] PREDEFINED_DIVISORS = { 1, 2, 3, 4, 6, 8, 12, 16 };

    public Bindable<BeatDivisorCollection> ValidDivisors { get; } = new Bindable<BeatDivisorCollection>(BeatDivisorCollection.COMMON);
    
    public BindableBeatDivisor(int value = 4)
        : base(value)
    {
        ValidDivisors.BindValueChanged(_ => updateBindableProperties(), true);
        BindValueChanged(_ => ensureValidDivisor());
    }

    public void SetArbitraryDivisor(int divisor)
    {
        if (!ValidDivisors.Value.Divisors.Contains(divisor))
        {
            if (BeatDivisorCollection.COMMON.Divisors.Contains(divisor))
                ValidDivisors.Value = BeatDivisorCollection.COMMON;
            else if (BeatDivisorCollection.TRIPLETS.Divisors.Contains(divisor))
                ValidDivisors.Value = BeatDivisorCollection.TRIPLETS;
            else
                ValidDivisors.Value = BeatDivisorCollection.Custom(divisor);
        }
        
        Value = divisor;
    }

    private void updateBindableProperties()
    {
        ensureValidDivisor();

        MinValue = ValidDivisors.Value.Divisors.Min();
        MaxValue = ValidDivisors.Value.Divisors.Max();
    }

    private void ensureValidDivisor()
    {
        if (!ValidDivisors.Value.Divisors.Contains(Value))
            Value = 1;
    }

    protected override int DefaultPrecision => 1;

    public override void BindTo(Bindable<int> them)
    {
        if (them is BindableBeatDivisor other)
            ValidDivisors.BindTo(other.ValidDivisors);
        
        base.BindTo(them);
    }

    protected override Bindable<int> CreateInstance() => new BindableBeatDivisor();

    public static int GetDivisorForBeatIndex(int index, int beatDivisor, int[]? validDivisors = null)
    {
        validDivisors ??= PREDEFINED_DIVISORS;

        var beat = index % beatDivisor;

        foreach (var divisor in validDivisors)
        {
            if ((beat * divisor) % beatDivisor == 0)
                return divisor;
        }
        
        return 0;
    }
}
