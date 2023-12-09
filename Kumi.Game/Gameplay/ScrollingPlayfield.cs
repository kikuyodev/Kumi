using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Gameplay;

public abstract partial class ScrollingPlayfield : Playfield
{
    protected ScrollingPlayfield(WorkingChart workingChart)
        : base(workingChart)
    {
    }
    
    public ScrollingNoteContainer ScrollContainer => (ScrollingNoteContainer) NoteContainer;

    protected override Container<DrawableNote> CreateNoteContainer() => new ScrollingNoteContainer();

    protected override double ComputeInitialLifetimeOffset(DrawableNote drawableNote)
    {
        var computed = ScrollContainer.ComputeLifetimeStart(drawableNote);
        return Math.Min(drawableNote.Note.StartTime - drawableNote.Note.Windows.WindowFor(NoteHitResult.Bad), computed);
    }
}
