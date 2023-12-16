using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Kumi.Game.Charts.Drawables;

public partial class DifficultyPill : CircularContainer
{
    protected readonly float Difficulty;

    public DifficultyPill(float difficulty)
    {
        Difficulty = difficulty;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Masking = true;

        Children = new[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = getDifficultyColor()
            },
            CreateContent()
        };
    }

    protected virtual Drawable CreateContent()
        => new SpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = Difficulty.ToString("N2"),
            Font = KumiFonts.GetFont(weight: FontWeight.Bold, size: 8),
            Padding = new MarginPadding
            {
                Vertical = 1,
                Horizontal = 8
            }
        };

    private Color4 getDifficultyColor()
    {
        // TODO
        return Colours.Gray(0.2f);
    }
}
