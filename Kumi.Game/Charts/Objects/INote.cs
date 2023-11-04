using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects.Windows;
using osuTK.Graphics;

namespace Kumi.Game.Charts.Objects;

public interface INote : IHasTime
{
    /// <summary>
    /// The type of <see cref="Note"/>.
    /// </summary>
    NoteType Type { get; set; }
    
    
    NoteFlags Flags { get; set; }

    /// <summary>
    /// The color of this note, for rendering.
    /// </summary>
    Color4 NoteColor { get; set; }
    
    /// <summary>
    /// The result timing windows for this note, as well as the result of the note.
    /// </summary>
    NoteWindows Windows { get; set; }
}

public enum NoteType : int
{
    Don = 0,
    Kat = 1,
    Drumroll = 2,
    Balloon = 3,
}

[Flags]
public enum NoteFlags
{
    None = 0,
    Big = 1 << 0,
}