using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Overlays.Chat;

public partial class ChannelListing : CompositeComponent
{
    private FillFlowContainer<DrawableChannelItem> channels = null!;

    public ChannelListing()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 200;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new[]
            {
                new Dimension(),
                new Dimension(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new BasicScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = channels = new FillFlowContainer<DrawableChannelItem>
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 2),
                            Padding = new MarginPadding
                            {
                                Top = 8
                            },
                        }
                    }
                },
                new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Padding = new MarginPadding
                        {
                            Bottom = 8
                        },
                        Children = new Drawable[]
                        {
                            new Circle
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 3,
                                Colour = Colours.Gray(0.08f),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Width = 0.8f
                            },
                            new DrawableChannelItem(false)
                            {
                                Icon = "+",
                                ChannelName = "Add Channel",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                            }
                        }
                    }
                }
            }
        };
        
        channels.Add(new DrawableChannelItem
        {
            Icon = "#",
            ChannelName = "Kumi"
        });
        
        channels.Add(new DrawableChannelItem
        {
            Icon = "#",
            ChannelName = "Modding"
        });
    }
}
