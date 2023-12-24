using Kumi.Game.Bindables;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Drawables.Parts;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Tests.Visual.Gameplay;

public partial class DrumHitPartTestScene : KumiTestScene
{
    [Test]
    public void TestParts()
    {
        AddStep("clear", Clear);
        AddStep("add grid", () =>
        {
            Add(new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new[]
                {
                    new Dimension(),
                    new Dimension()
                },
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension()
                },
                Content = new[]
                {
                    new[]
                    {
                        createContainer(new DrumHitPart(new LazyBindable<NoteType>(NoteType.Don))),
                        createContainer(new DrumHitPart(new LazyBindable<NoteType>(NoteType.Kat))),
                    },
                    new[]
                    {
                        createContainer(new DrumHitPart(new LazyBindable<NoteType>(NoteType.Don))),
                        createContainer(new DrumHitPart(new LazyBindable<NoteType>(NoteType.Kat))),
                    }
                }
            });
        });
    }

    private Drawable createContainer(Drawable drawable)
        => new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fit,
            FillAspectRatio = 1,
            Size = new Vector2(0.6f),
            Child = drawable.With(d =>
            {
                d.Anchor = Anchor.Centre;
                d.Origin = Anchor.Centre;
                d.RelativeSizeAxes = Axes.Both;
            })
        };
}
