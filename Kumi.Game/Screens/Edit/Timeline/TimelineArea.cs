using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineArea : CompositeDrawable
{
    private Timeline timeline = null!;

    public TimelineArea()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Masking = true,
                    CornerRadius = 5,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Depth = float.MaxValue,
                            Colour = Color4Extensions.FromHex("0D0D0D")
                        },
                        timeline = new Timeline()
                    }
                },
                new Container
                {
                    Name = "Cursor",
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Rotation = 45,
                            Colour = Color4Extensions.FromHex("FF0040"),
                            Size = new Vector2(6)
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 1.25f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = Color4Extensions.FromHex("FF0040"),
                            EdgeSmoothness = new Vector2(1f, 0)
                        },
                        new Box
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.Centre,
                            Rotation = 45,
                            Colour = Color4Extensions.FromHex("FF0040"),
                            Size = new Vector2(6)
                        }
                    }
                }
            }
        };
    }
}
