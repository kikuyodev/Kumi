using System.Diagnostics;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Drawables.Parts;
using Kumi.Game.Input;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace Kumi.Game.Gameplay.Drawables;

public partial class DrawableDrumHit : DrawableNote<DrumHit>, IKeyBindingHandler<GameplayAction>
{
    public static readonly ColourInfo DON_COLOUR_GRADIENT = ColourInfo.GradientVertical(
        Color4Extensions.FromHex("F96226"),
        Color4Extensions.FromHex("F94926")
    );
    
    public static readonly ColourInfo KAT_COLOUR_GRADIENT = ColourInfo.GradientVertical(
        Color4Extensions.FromHex("68C0C1"),
        Color4Extensions.FromHex("6894C1")
    );
    
    private readonly Drawable? drumHitPart;
    
    public DrawableDrumHit(DrumHit note)
        : base(note)
    {
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;
        
        AddInternal(drumHitPart = createDrawable(new DrumHitPart(note.Type)));
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();
        Width = DrawSize.Y;
    }

    private Drawable createDrawable(Drawable drawable)
        => drawable.With(d =>
        {
            d.Anchor = Anchor.Centre;
            d.Origin = Anchor.Centre;
            d.RelativeSizeAxes = Axes.Both;
        });

    protected override void UpdateHitStateTransforms(NoteState newState)
    {
        switch (newState)
        {
            case NoteState.Hit:
                drumHitPart.MoveToY(-100, 250, Easing.OutBack);
                drumHitPart.FadeOut(250, Easing.OutQuint);
                
                this.Delay(250).Expire();
                break;
            case NoteState.Miss:
                drumHitPart!.FadeColour(Color4Extensions.FromHex("0D0D0D").Opacity(0.5f), 100, Easing.OutQuint);
                drumHitPart.FadeOut(100);

                this.Delay(100).Expire();
                break;
        }
    }

    protected override void CheckForResult(bool userTriggered, double deltaTime)
    {
        Debug.Assert(Note.Windows != null);

        if (!userTriggered)
        {
            if (Time.Current > Note.Time - Note.Windows.WindowFor(NoteHitResult.Bad) && !Note.Windows.IsWithinWindow(deltaTime))
                ApplyResult(NoteHitResult.Miss);
            
            return;
        }

        var result = Note.Windows.ResultFor(deltaTime);
        if (result == null)
            return;
        
        ApplyResult(result.Value);
    }

    public bool OnPressed(KeyBindingPressEvent<GameplayAction> e)
    {
        if (Judged)
            return false;

        switch (Note.Type)
        {
            case NoteType.Don:
                if (e.Action is GameplayAction.RightCentre or GameplayAction.LeftCentre)
                    return UpdateResult(true);
                break;
            case NoteType.Kat:
                if (e.Action is GameplayAction.RightRim or GameplayAction.LeftRim)
                    return UpdateResult(true);
                break;
        }
        
        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GameplayAction> e)
    {
    }
}
