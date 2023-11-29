using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Algorithms;
using Kumi.Game.Gameplay.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Testing;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace Kumi.Tests.Visual.Gameplay;

public partial class ScrollingPlayfieldTestScene : PlayfieldTestScene
{
    protected override Playfield CreatePlayfield(WorkingChart workingChart) => new TestPlayfield(workingChart);

    protected override void CreateAsserts()
    {
        AddAssert("note is physically closer", () =>
        {
            var note = Playfield!.ChildrenOfType<DrawableNote>().FirstOrDefault();
            if (note == null) return false;
            
            return note.DrawWidth <= Playfield!.NoteContainerInternal.DrawWidth / 2;
        });
    }

    private partial class TestPlayfield : ScrollingPlayfield
    {
        private readonly Bindable<IScrollAlgorithm> algorithm = new Bindable<IScrollAlgorithm>(new LinearScrollAlgorithm());
        
        public TestPlayfield(WorkingChart workingChart)
            : base(workingChart)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 200,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativePositionAxes = Axes.Both,
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.8f,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Y,
                                Width = 2,
                                Colour = Color4.White
                            },
                            NoteContainer
                        }
                    }
                }
            };

            ScrollContainer.Algorithm.BindTo(algorithm);
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
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;

            AddInternal(circle = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Color4.White
            });
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
                if (Time.Current > Note.StartTime - Note.Windows.WindowFor(NoteHitResult.Bad) && !Note.Windows.IsWithinWindow(deltaTime))
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
