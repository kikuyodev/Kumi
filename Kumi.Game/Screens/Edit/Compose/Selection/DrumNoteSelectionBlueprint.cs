using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Screens.Edit.Compose.Pieces;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Compose.Selection;

public partial class DrumNoteSelectionBlueprint : NoteSelectionBlueprint
{
    public new DrawableDrumHit DrawableNote => (DrawableDrumHit) base.DrawableNote;

    public DrumNoteSelectionBlueprint(Note item)
        : base(item)
    {
        RelativeSizeAxes = Axes.None;
        InternalChild = new HitPiece
        {
            Size = new Vector2(0.8f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat || !IsSelected)
            return false;

        switch (e.Key)
        {
            case Key.Number1:
                DrawableNote.Note.Type.Value = NoteType.Don;
                return true;

            case Key.Number2:
                DrawableNote.Note.Type.Value = NoteType.Kat;
                return true;
        }

        return false;
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
