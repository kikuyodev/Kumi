using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osuTK.Graphics;

namespace Kumi.Game.Charts.Objects;

/// <summary>
/// A representation of a clickable object in a <see cref="IChart" />.
/// A majority of the properties are bindable, so that they can be used in the editor.
/// </summary>
public class Note : INote
{
    /// <summary>
    /// The scale or a note with <see cref="NoteFlags.Big" /> set.
    /// </summary>
    public const float BIG_NOTE_SCALE = 0.0f;

    /// <summary>
    /// The delimiter used to split the input string.
    /// </summary>
    public const char DELIMITER = ',';

    public NoteType Type
    {
        get => typeBindable.Value;
        set => typeBindable.Value = value;
    }

    public NoteFlags Flags
    {
        get => flagsBindable.Value;
        set => flagsBindable.Value = value;
    }

    public double Time
    {
        get => startTimeBindable.Value;
        set => startTimeBindable.Value = value;
    }

    public Color4 NoteColor
    {
        get => noteColorBindable.Value;
        set => noteColorBindable.Value = value;
    }

    // TODO: Initialize this somewhere, we're not guaranteed to have a window for every note,
    //       especially when we're just decoding the chart.
    public NoteWindows Windows { get; set; } = null!;

    protected Note(float startTime)
    {
        Time = startTime;
        typeBindable.ValueChanged += v => NoteColor = getColorFromType(v.NewValue);
    }

    private readonly Bindable<NoteType> typeBindable = new Bindable<NoteType>();
    private readonly Bindable<double> startTimeBindable = new Bindable<double>();
    private readonly Bindable<Color4> noteColorBindable = new Bindable<Color4>();
    private readonly Bindable<NoteFlags> flagsBindable = new Bindable<NoteFlags>();

    /// <summary>
    /// A function that applies various defaults to an object within a <see cref="IChart" />.
    /// This is primarily for figuring out which of the two windows to use for a note, between
    /// an upper range and a lower range. The ranges for difficulty are defined in <see cref="NoteWindows" />.
    /// </summary>
    /// <param name="chart"></param>
    public void ApplyChartDefaults(IChart chart)
    {
        // TODO: Set windows based on difficulty rating.
        Windows.ApplyDifficultyRating(0.0f);
    }

    /// <summary>
    /// Gets the total duration of this note.
    /// ...I'm not sure if this is useful, but it's here just in case.
    /// </summary>
    public double GetDuration() => this is IHasEndTime ? (this as IHasEndTime)!.EndTime - Time : 0.0f;

    private Color4 getColorFromType(NoteType type)
    {
        switch (type)
        {
            case NoteType.Don:
                return Colors.DonColor;

            case NoteType.Kat:
                return Colors.KatColor;
        }

        return Color4.White;
    }
}
