using Kumi.Game.Charts.Objects.Windows;

namespace Kumi.Game.Extensions;

public static class NoteHitResultExtensions
{
    public static bool AffectsScore(this NoteHitResult result)
    {
        if (result != NoteHitResult.None && result != NoteHitResult.Miss)
            return true;
        
        return false;
    }
}
