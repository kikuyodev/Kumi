using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osuTK.Graphics;

namespace Kumi.Game.Charts.Objects;

/// <summary>
/// A representation of a clickable object in a <see cref="IChart"/>.
///
/// A majority of the properties are bindable, so that they can be used in the editor.
/// </summary>
public class Note : INote, IHasTime
{
    /// <summary>
    /// The scale or a note with <see cref="NoteFlags.Big"/> set.
    /// </summary>
    public const float BIG_NOTE_SCALE = 0.0f;

    public NoteType Type
    {
        get => _typeBindable.Value;
        set => _typeBindable.Value = value;
    }

    public NoteFlags Flags
    {
        get => _flagsBindable.Value;
        set => _flagsBindable.Value = value;
    }

    public float StartTime
    {
        get => _startTimeBindable.Value;
        set => _startTimeBindable.Value = value;
    }

    public Color4 NoteColor
    {
        get => _noteColorBindable.Value;
        set => _noteColorBindable.Value = value;
    }

    public NoteWindows Windows { get; set; }

    public Note()
    {
        _typeBindable.ValueChanged += v => NoteColor = getColorFromType(v.NewValue);
    }

    private Bindable<NoteType> _typeBindable = new();
    private Bindable<float> _startTimeBindable = new();
    private Bindable<Color4> _noteColorBindable = new();
    private Bindable<NoteFlags> _flagsBindable = new();
    
    /// <summary>
    /// A function that applies various defaults to an object within a <see cref="IChart"/>.
    ///
    /// This is primarily for figuring out which of the two windows to use for a note, between
    /// an upper range and a lower range. The ranges for difficulty are defined in <see cref="NoteWindows"/>.
    /// </summary>
    /// <param name="chart"></param>
    public void ApplyChartDefaults(IChart chart)
    {
        // TODO: Set windows based on difficulty rating.
        Windows.ApplyDifficultyRating(0.0f);
    }

    /// <summary>
    /// Gets the total duration of this note.
    ///
    /// ...I'm not sure if this is useful, but it's here just in case.
    /// </summary>
    public float GetDuration() => this is IHasEndTime ? (this as IHasEndTime)!.EndTime - StartTime : 0.0f;

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
