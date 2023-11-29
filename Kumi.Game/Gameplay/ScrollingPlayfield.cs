using Kumi.Game.Charts;
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
}
