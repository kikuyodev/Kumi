using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osuTK.Graphics;

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

    public static Color4 GetColourFor(int divisor)
    {
        switch (divisor)
        {
            case 1:
                return Color4.White;

            case 2:
                return Color4Extensions.FromHex("FF3366");

            case 4:
                return Color4Extensions.FromHex("33BBFF");

            case 8:
                return Color4Extensions.FromHex("FFD333");

            case 16:
                return Color4Extensions.FromHex("8C66FF").Opacity(0.5f);

            case 3:
                return Color4Extensions.FromHex("6633FF");

            case 6:
                return Color4Extensions.FromHex("FF7033");

            case 12:
                return Color4Extensions.FromHex("8C66FF").Opacity(0.5f);

            default:
                return Color4Extensions.FromHex("FF3366");
        }
    }

    public static float GetHeightFor(int divisor)
    {
        switch (divisor)
        {
            case 1:
            case 2:
                return 0.8f;

            case 3:
            case 4:
                return 0.6f;

            case 6:
            case 8:
                return 0.5f;

            default:
                return 0.4f;
        }
    }

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
