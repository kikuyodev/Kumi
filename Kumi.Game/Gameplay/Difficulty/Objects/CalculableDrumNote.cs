using Kumi.Game.Charts.Objects;

namespace Kumi.Game.Gameplay.Difficulty.Objects;

/// <summary>
/// A calculable drum note, which acts as a wrapper for note types such as <see cref="NoteType.Kat" /> and
/// <see cref="NoteType.Don" />
/// </summary>
public class CalculableDrumNote : CalculableNote
{
    public CalculableDrumNote(INote currentNote, List<CalculableNote> notes, int index)
        : base(currentNote, notes, index)
    {
    }
}
