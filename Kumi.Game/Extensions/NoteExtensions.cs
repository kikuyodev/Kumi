using Kumi.Game.Charts.Objects;

namespace Kumi.Game.Extensions;

public static class NoteExtensions
{
    public static string ToSaveableString(this Note note)
    {
        if (note is DrumHit hit)
        {
            return $"{(int)hit.Type.Value}{Note.DELIMITER}{hit.StartTime}{Note.DELIMITER}{(int)hit.Flags.Value}";
        }

        return "";
    }
    
}
