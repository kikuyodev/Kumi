namespace Kumi.Game.Charts.Objects;

/// <summary>
/// A note that can be hit, typically a <see cref="NoteType.Don"/> or <see cref="NoteType.Kat"/>.
/// </summary>
// We don't need to implement anything here, since we're just inheriting from Note.
public class DrumHit : Note
{
    public DrumHit(float startTime)
        : base(startTime)
    {
    }
}
