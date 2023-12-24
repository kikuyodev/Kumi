using Kumi.Game.Charts.Objects;

namespace Kumi.Game.Extensions;

public static class NoteExtensions
{
    public static string ToSaveableString(this Note note)
    {
        if (note is DrumHit hit)
        {
            return $"{(int)hit.Type.Value}{Note.DELIMITER}{hit.StartTime}{Note.DELIMITER}{(int)hit.Flags.Value}";
        } else if (note is DrumRoll roll)
        {
            return $"{(int)roll.Type.Value}{Note.DELIMITER}{roll.StartTime}{Note.DELIMITER}{roll.EndTime}{Note.DELIMITER}{(int)roll.Flags.Value}";
        } else if (note is Balloon balloon)
        {
            return $"{(int)balloon.Type.Value}{Note.DELIMITER}{balloon.StartTime}{Note.DELIMITER}{balloon.EndTime}{Note.DELIMITER}{(int)balloon.Flags.Value}";
        }

        return "";
    }
    
}
