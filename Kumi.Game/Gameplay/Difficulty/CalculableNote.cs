using Kumi.Game.Charts.Objects;

namespace Kumi.Game.Gameplay.Difficulty;

/// <summary>
/// A wrapper for a note that can be used in a <see cref="WorkingDifficultyCalculator" />
/// </summary>
public class CalculableNote
{
    /// <summary>
    /// The current index of the note in the calculable notes.
    /// </summary>
    public int Index { get; set; }

    public INote CurrentNote { get; set; }

    /// <summary>
    /// Gets the start time of the note.
    /// </summary>
    public double StartTime => CurrentNote.StartTime;

    public CalculableNote(INote currentNote, List<CalculableNote> notes, int index)
    {
        CurrentNote = currentNote;
        calculableNotes = notes;
        Index = index;
    }

    private List<CalculableNote> calculableNotes { get; }

    /// <summary>
    /// Gets the next note in the list of calculable notes.
    /// </summary>
    /// <param name="idx">The index.</param>
    public CalculableNote? Next(int idx) => calculableNotes.ElementAtOrDefault(Index + idx);

    /// <summary>
    /// Gets the previous note in the list of calculable notes.
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public CalculableNote? Previous(int idx) => calculableNotes.ElementAtOrDefault(Index - idx);

    /// <summary>
    /// Gets the number of calculable notes that are in the list.
    /// </summary>
    public int Count() => calculableNotes.Count;
}
