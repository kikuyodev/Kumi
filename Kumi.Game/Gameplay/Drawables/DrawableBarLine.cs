using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Judgements;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay.Drawables;

public partial class DrawableBarLine : DrawableNote<BarLine>
{
    public DrawableBarLine(BarLine note)
        : base(note)
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;

        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        AddRangeInternal(new[]
        {
            new Circle
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Width = 3,
                RelativeSizeAxes = Axes.Y,
                Height = 0.8f,
                Colour = Colour4.White.Opacity(note.Major.Value ? 0.3f : 0.1f)
            },
            note.Major.Value
                ? new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Circle
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Y = 104,
                            Width = 3,
                            RelativeSizeAxes = Axes.Y,
                            Height = 0.5f,
                            Colour = ColourInfo.GradientVertical(Color4.White.Opacity(0.5f), Color4.White.Opacity(0f))
                        },
                        new Circle
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Y = -104,
                            Width = 3,
                            RelativeSizeAxes = Axes.Y,
                            Height = 0.5f,
                            Colour = ColourInfo.GradientVertical(Color4.White.Opacity(0f), Color4.White.Opacity(0.5f))
                        },
                    }
                }
                : Empty()
        });
    }

    protected override Judgement CreateJudgement()
        => new BarLineJudgement(Note);

    protected override void UpdateHitStateTransforms(NoteState newState)
    {
        using (BeginAbsoluteSequence(Note.StartTime))
            this.FadeOutFromOne(150).Expire();
    }
}
