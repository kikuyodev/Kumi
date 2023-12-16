using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Screens.Edit.Compose.Pieces;
using osu.Framework.Graphics;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose.Selection;

public partial class BigDrumNoteSelectionBlueprint : NoteSelectionBlueprint
{
    public new DrawableBigDrumHit DrawableNote => (DrawableBigDrumHit) base.DrawableNote;

    public BigDrumNoteSelectionBlueprint(Note item)
        : base(item)
    {
        RelativeSizeAxes = Axes.None;
        InternalChild = new HitPiece
        {
            Size = new Vector2(1.1f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    protected override void Update()
    {
        base.Update();

        var topLeft = new Vector2(float.MaxValue, float.MaxValue);
        var bottomRight = new Vector2(float.MinValue, float.MinValue);

        topLeft = Vector2.ComponentMin(topLeft, Parent!.ToLocalSpace(DrawableNote.DrumHitPart!.ScreenSpaceDrawQuad.TopLeft));
        bottomRight = Vector2.ComponentMax(bottomRight, Parent!.ToLocalSpace(DrawableNote.DrumHitPart!.ScreenSpaceDrawQuad.BottomRight));

        Size = bottomRight - topLeft;
        Position = topLeft;
    }
}
