using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Drawables.Parts;
using Kumi.Game.Input;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay.Drawables;

public partial class DrawableDrumRoll : DrawableNote<DrumRoll>, IKeyBindingHandler<GameplayAction>
{
    private const int required_hits = 3;
    private int hits;

    private readonly Container content;
    private readonly YellowCirclePart corePart;
    private readonly CircularContainer spanPart;

    public DrawableDrumRoll(DrumRoll note)
        : base(note)
    {
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        AddInternal(content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Children = new Drawable[]
            {
                spanPart = new CircularContainer
                {
                    Masking = true,
                    MaskingSmoothness = 1f,
                    BorderColour = Color4.White,
                    BorderThickness = 6.5f,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(1f, 0.7f * 0.75f),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = DrawableBalloon.BALLOON_COLOUR_GRADIENT
                    }
                },
                corePart = new YellowCirclePart
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Y,
                    Height = 0.7f
                },
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Note.Flags.Bindable.BindValueChanged(f =>
        {
            if (f.NewValue.HasFlagFast(NoteFlags.Big))
            {
                corePart.Height = 1f;
                spanPart.Height = 0.75f;
            }
            else
            {
                corePart.Height = 0.7f;
                spanPart.Height = 0.7f * 0.75f;
            }
        }, true);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();
        corePart.Width = corePart.DrawHeight;

        content.X = content.DrawHeight / 2;
    }

    protected override void CheckForResult(bool userTriggered, double deltaTime)
    {
        if (userTriggered)
        {
            hits++;
            ApplyBonusResult(j => j.ApplyResult(NoteHitResult.Good, Time.Current));
        }
        else
        {
            if (deltaTime < 0)
                return;

            var result = hits >= required_hits ? NoteHitResult.Good : NoteHitResult.Miss;
            ApplyResult(result);
        }
    }

    protected override void UpdateHitStateTransforms(NoteState newState)
    {
        switch (newState)
        {
            case NoteState.Miss:
            case NoteState.Hit:
                this.FadeOut(300, Easing.OutQuint);
                break;
        }
    }

    public bool OnPressed(KeyBindingPressEvent<GameplayAction> e)
    {
        if (Judged)
            return false;

        if (Time.Current < Note.StartTime)
            return false;

        if (Note.Flags.Value.HasFlagFast(NoteFlags.Big))
            return handleBigInput(e);

        UpdateResult(true);
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<GameplayAction> e)
    {
    }

    private GameplayAction? previousAction;
    private bool? isRim;
    private bool? lastResult;
    
    private bool handleBigInput(KeyBindingPressEvent<GameplayAction> e)
    {
        if (lastResult.HasValue)
        {
            if (lastResult.Value && e.Action is GameplayAction.RightRim or GameplayAction.LeftRim)
                return false;

            if (!lastResult.Value && e.Action is GameplayAction.RightCentre or GameplayAction.LeftCentre)
                return false;
        }
        
        // Make the user press the same key twice to hit a big note, alternating between the centre and the edge.
        if (previousAction == null)
        {
            previousAction = e.Action;
            isRim = e.Action is GameplayAction.RightRim or GameplayAction.LeftRim;
            return false;
        }

        if (previousAction == e.Action)
            return false;
        
        if (isRim!.Value)
        {
            if ((previousAction != GameplayAction.RightRim || e.Action != GameplayAction.LeftRim) &&
                (previousAction != GameplayAction.LeftRim || e.Action != GameplayAction.RightRim))
                return false;

            previousAction = null;
            isRim = null;
            lastResult = true;
            return UpdateResult(true);
        }
        else
        {
            if ((previousAction != GameplayAction.RightCentre || e.Action != GameplayAction.LeftCentre) &&
                (previousAction != GameplayAction.LeftCentre || e.Action != GameplayAction.RightCentre))
                return false;

            previousAction = null;
            isRim = null;
            lastResult = false;
            return UpdateResult(true);
        }
    }
}
