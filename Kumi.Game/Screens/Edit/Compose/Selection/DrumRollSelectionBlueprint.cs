using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Screens.Edit.Compose.Pieces;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace Kumi.Game.Screens.Edit.Compose.Selection;

public partial class DrumRollSelectionBlueprint : NoteSelectionBlueprint<DrumRoll>
{
    public new DrawableDrumRoll DrawableNote => (DrawableDrumRoll) base.DrawableNote;
    
    public DrumRollSelectionBlueprint(DrumRoll item)
        : base(item)
    {
        RelativeSizeAxes = Axes.None;
        Padding = new MarginPadding { Right = -24 };
        InternalChild = new HitPiece
        {
            Size = new Vector2(1f, 0.8f),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        };
    }

    protected override void Update()
    {
        base.Update();

        var topLeft = new Vector2(float.MaxValue, float.MaxValue);
        var bottomRight = new Vector2(float.MinValue, float.MinValue);

        var rect = DrawableNote.Content.ScreenSpaceDrawQuad.AABBFloat;
        rect = RectangleF.Union(rect, DrawableNote.CorePart.ScreenSpaceDrawQuad.AABBFloat);

        topLeft = Vector2.ComponentMin(topLeft, Parent!.ToLocalSpace(rect.TopLeft));
        bottomRight = Vector2.ComponentMax(bottomRight, Parent!.ToLocalSpace(rect.BottomRight));

        Size = bottomRight - topLeft;
        Position = topLeft - new Vector2(8, 0);
    }
}
