using Kumi.Game.Charts.Objects;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay.Drawables.Parts;

public partial class BigDrumHitPart : ConstrictedScalingContainer
{
    private readonly NoteType noteType;
    
    public BigDrumHitPart(NoteType noteType)
    {
        this.noteType = noteType;
        PreferredSize = new Vector2(72);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new CircularContainer
        {
            Masking = true,
            BorderColour = Color4.White,
            BorderThickness = 5,
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true
                },
                new CircularProgress
                {
                    Scale = new Vector2(0.8f),
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    InnerRadius = 0.675f,
                    Current = { Value = 1f },
                    Colour = noteType == NoteType.Kat ? DrawableDrumHit.KAT_COLOUR_GRADIENT : DrawableDrumHit.DON_COLOUR_GRADIENT,
                },
                new Circle
                {
                    Scale = new Vector2(0.2f),
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            }
        };
    }
}
