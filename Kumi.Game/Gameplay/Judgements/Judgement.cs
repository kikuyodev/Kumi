using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;

namespace Kumi.Game.Gameplay.Judgements;

public class Judgement
{
    public NoteHitResult Result;

    public readonly INote Note;
    public readonly bool IsBonus;

    public bool Ignored { get; set; }
    
    internal double? RawTime { get; set; }

    public double? AbsoluteTime => RawTime != null ? Math.Min(RawTime.Value, Note.GetEndTime() + Note.Windows.WindowFor(NoteHitResult.Bad)) : Note.GetEndTime();

    public double DeltaTime => AbsoluteTime ?? 0 - Note.GetEndTime();

    public bool HasResult => Result > NoteHitResult.None;

    public bool IsHit => Result > NoteHitResult.Miss;

    public Judgement(INote note, bool isBonus = false)
    {
        Note = note;
        IsBonus = isBonus;
    }

    public void ApplyResult(NoteHitResult result, double currentTime)
    {
        Result = result;
        RawTime = currentTime;
    }

    internal void Reset()
    {
        Result = NoteHitResult.None;
        RawTime = null;
    }
}
