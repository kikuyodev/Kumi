using Kumi.Game.Bindables;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay.Drawables.Parts;

public partial class DrumHitPart : ConstrictedScalingContainer
{
    private readonly LazyBindable<NoteType> noteType;

    public DrumHitPart(LazyBindable<NoteType> noteType)
    {
        this.noteType = noteType;
        PreferredSize = new Vector2(72);
    }
    
    private Box colourBox = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new CircularContainer
        {
            Masking = true,
            BorderColour = Color4.White,
            BorderThickness = 5,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.7f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Child = colourBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
            }
        };
        
        noteType.Bindable.BindValueChanged(v =>
        {
            if (v.NewValue == NoteType.Kat)
                colourBox.Colour = DrawableDrumHit.KAT_COLOUR_GRADIENT;
            else
                colourBox.Colour = DrawableDrumHit.DON_COLOUR_GRADIENT;
        }, true);
    }
}
