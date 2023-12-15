using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay.Drawables.Parts;
using Kumi.Game.Input;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace Kumi.Game.Gameplay.Drawables;

public partial class DrawableBalloon : DrawableNote<Balloon>, IKeyBindingHandler<GameplayAction>
{
    public static readonly ColourInfo BALLOON_COLOUR_GRADIENT = ColourInfo.GradientVertical(
        Color4Extensions.FromHex("F9CB26"),
        Color4Extensions.FromHex("F9A826")
    );

    private const int required_hits = 3;
    private int hits;

    private readonly BalloonCirclePart balloonPart;

    public DrawableBalloon(Balloon note)
        : base(note)
    {
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        AddInternal(new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Y,
            AutoSizeAxes = Axes.X,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(8),
            X = 18,
            Children = new Drawable[]
            {
                new BalloonCirclePart
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Y,
                    Height = 0.7f
                },
                new Circle
                {
                    Height = 5,
                    Width = 26,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                balloonPart = new BalloonCirclePart(false)
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Y,
                    Height = 0.7f,
                    Scale = new Vector2(0f),
                    AlwaysPresent = true
                }
            }
        });
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();
        Width = DrawSize.Y;
    }

    protected override void CheckForResult(bool userTriggered, double deltaTime)
    {
        if (userTriggered)
        {
            hits++;

            balloonPart.ScaleTo(Math.Clamp((float) hits / required_hits, 0f, 1f), 100, Easing.OutQuint);

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

    protected override void Update()
    {
        base.Update();

        X = Math.Max(0, X);
    }

    public bool OnPressed(KeyBindingPressEvent<GameplayAction> e)
    {
        if (Judged)
            return false;

        if (Time.Current < Note.StartTime)
            return false;

        if (e.Action is not (GameplayAction.RightCentre or GameplayAction.LeftCentre))
            return false;

        UpdateResult(true);
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<GameplayAction> e)
    {
    }
}
