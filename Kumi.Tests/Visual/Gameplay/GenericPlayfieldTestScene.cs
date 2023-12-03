using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace Kumi.Tests.Visual.Gameplay;

public partial class GenericPlayfieldTestScene : PlayfieldTestScene
{
    protected override Playfield CreatePlayfield(WorkingChart workingChart) => new TestPlayfield(workingChart);

    private partial class TestPlayfield : Playfield
    {
        public TestPlayfield(WorkingChart workingChart)
            : base(workingChart)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                NoteContainer
            };
        }

        protected override DrawableNote CreateDrawableNote(INote note)
        {
            if (note is TestNote testNote)
                return new TestDrawableNote(testNote);

            throw new ArgumentException("Note is not a TestNote", nameof(note));
        }
    }
    
    private partial class TestDrawableNote : DrawableNote<TestNote>
    {
        private Circle circle = null!;

        public TestDrawableNote(TestNote note)
            : base(note)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(100);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Position = new Vector2(RNG.NextSingle(-200, 200), RNG.NextSingle(-200, 200));

            AddInternal(circle = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Color4.White
            });
        }

        protected override void InitialTransforms()
        {
            circle.ScaleTo(0.5f).Then().ScaleTo(1f, 1000 - Note.Windows.WindowFor(NoteHitResult.Bad), Easing.OutQuint);
            circle.FadeIn(1000 - Note.Windows.WindowFor(NoteHitResult.Bad)).Then().FadeColour(Color4.Blue, Note.Windows.WindowFor(NoteHitResult.Bad));
        }

        protected override void UpdateHitStateTransforms(NoteState newState)
        {
            switch (newState)
            {
                case NoteState.Hit:
                    circle.ScaleTo(1.5f).Then().ScaleTo(1f, 1000);
                    circle.FadeColour(Color4.Lime, 250, Easing.OutQuint);
                    circle.FadeOut(250);

                    this.Delay(250).Expire();
                    break;

                case NoteState.Miss:
                    circle.FadeColour(Color4.White).Then().FadeColour(Color4.Red, 250, Easing.OutQuint);
                    circle.FadeOut(250);

                    this.Delay(250).Expire();
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

            var result = Note.Windows.ResultFor(deltaTime)!;
            if (result == null)
                return;

            ApplyResult(result.Value);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (Judged)
                return false;

            if ((e.Key == Key.F || e.Key == Key.J) && !e.Repeat)
                return UpdateResult(true);

            return false;
        }
    }
}
