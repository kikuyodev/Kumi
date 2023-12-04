using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
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
        InternalChild = new FillFlowContainer
        {
            Direction = FillDirection.Vertical,
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(0, 4),
            Children = new Drawable[]
            {
                new Container
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
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(4, 0),
                            Children = new Drawable[]
                            {
                                new KumiIconButton
                                {
                                    BackgroundColour = Color4Extensions.FromHex("0D0D0D"),
                                    IconColour = Color4Extensions.FromHex("CCCCCC"),
                                    Size = new Vector2(20),
                                    Icon = FontAwesome.Solid.SearchPlus,
                                    IconScale = new Vector2(0.5f),
                                    Action = () => timeline.AdjustZoomRelatively(1)
                                },
                                new KumiIconButton
                                {
                                    BackgroundColour = Color4Extensions.FromHex("0D0D0D"),
                                    IconColour = Color4Extensions.FromHex("CCCCCC"),
                                    Size = new Vector2(20),
                                    Icon = FontAwesome.Solid.SearchMinus,
                                    IconScale = new Vector2(0.5f),
                                    Action = () => timeline.AdjustZoomRelatively(-1)
                                }
                            }
                        },
                        new BeatDivisorControl
                        {
                            Size = new Vector2(173, 50),
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                        }
                    }
                }
            }
        };
    }
}
