using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Screens.Edit.Blueprints;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose;

public abstract partial class NoteSelectionBlueprint : SelectionBlueprint<Note>
{
    public DrawableNote DrawableNote { get; internal set; } = null!;

    protected override bool ShouldBeAlive => (DrawableNote?.IsAlive == true && DrawableNote.IsPresent) || (State == SelectionState.Selected);

    protected NoteSelectionBlueprint(Note item)
        : base(item)
    {
    }

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        => DrawableNote.ReceivePositionalInputAt(screenSpacePos);

    public override Quad SelectionQuad
        => DrawableNote.ScreenSpaceDrawQuad;
}

public abstract partial class NoteSelectionBlueprint<T> : NoteSelectionBlueprint
    where T : Note
{
    protected new T Item => (T)base.Item;

    protected NoteSelectionBlueprint(T item)
        : base(item)
    {
    }
}
