using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;

namespace Kumi.Game.Gameplay.Judgements;

public class Judgement
{
    public NoteHitResult Result;

    public readonly INote Note;

    public double? AbsoluteTime { get; set; }

    public double DeltaTime => AbsoluteTime ?? 0 - Note.StartTime;

    public bool HasResult => Result > NoteHitResult.None;

    public bool IsHit => Result > NoteHitResult.Miss;

    public Judgement(INote note)
    {
        Note = note;
    }

    public void ApplyResult(NoteHitResult result, double currentTime)
    {
        Result = result;
        AbsoluteTime = currentTime;
    }

    internal void Reset()
    {
        Result = NoteHitResult.None;
        AbsoluteTime = null;
    }

    public int GetScore()
    {
        if (!HasResult)
            return 0;

        return Result switch
        {
            NoteHitResult.Miss => 0,
            NoteHitResult.Bad => 100,
            NoteHitResult.Ok => 200,
            NoteHitResult.Good => 300,
            _ => 0
        };
    }
}
