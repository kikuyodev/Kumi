using System.Collections.Specialized;
using System.Diagnostics;
using Kumi.Game.Graphics;
using Kumi.Game.Online;
using Kumi.Game.Online.Channels;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Overlays.Chat;

public partial class ChannelListing : CompositeComponent
{
    [Resolved]
    private ChannelManager channelsManager { get; set; } = null!;

    public ChannelListing()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 200;
    }

    private FillFlowContainer<DrawableChannelItem> channelsFlow = null!;

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
                        Child = channelsFlow = new FillFlowContainer<DrawableChannelItem>
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
                            new DrawableChannelItem(null, false)
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
        
        channelsManager.Channels.BindCollectionChanged(onChannelsChanged, true);
    }

    private void onChannelsChanged(object? _, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                channelsFlow.Clear();
                break;
            
            case NotifyCollectionChangedAction.Add:
                Debug.Assert(args.NewItems != null);
                
                foreach (var item in args.NewItems)
                {
                    var channel = (Channel) item;
                    channelsFlow.Add(new DrawableChannelItem(channel) { Icon = "#" });
                }
                break;
            
            case NotifyCollectionChangedAction.Remove:
                Debug.Assert(args.OldItems != null);
                
                foreach (var item in args.OldItems)
                {
                    var channel = (Channel) item;
                    channelsFlow.RemoveAll(c => c.Channel == channel, true);
                }
                break;
        }
    }
}
