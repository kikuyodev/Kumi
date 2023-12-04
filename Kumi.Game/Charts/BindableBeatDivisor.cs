using Kumi.Game.Graphics;
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

    public void SelectNext()
    {
        var divisors = ValidDivisors.Value.Divisors;
        if (divisors.Cast<int?>().SkipWhile(d => d != Value).ElementAtOrDefault(1) is { } newValue)
            Value = newValue;
    }

    public int NextDivisor(int? offset = null)
    {
        offset ??= Value;

        var divisors = ValidDivisors.Value.Divisors;
        return divisors.Cast<int?>().SkipWhile(d => d != offset).ElementAtOrDefault(1) ?? Value;
    }

    public void SelectPrevious()
    {
        var divisors = ValidDivisors.Value.Divisors;
        if (divisors.Cast<int?>().TakeWhile(d => d != Value).LastOrDefault() is { } newValue)
            Value = newValue;
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
                return Colours.RED_ACCENT;

            case 4:
                return Colours.CYAN_ACCENT;

            case 8:
                return Colours.YELLOW_ACCENT;

            case 16:
                return Colours.PURPLE_ACCENT_LIGHT.Opacity(0.5f);

            case 3:
                return Colours.PURPLE_ACCENT_LIGHT;

            case 6:
                return Colours.ORANGE_ACCENT;

            case 12:
                return Colours.PURPLE_ACCENT_LIGHTER.Opacity(0.5f);

            default:
                return Colours.RED_ACCENT;
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
