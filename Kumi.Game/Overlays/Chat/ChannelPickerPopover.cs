using System.Collections.Specialized;
using System.Diagnostics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online;
using Kumi.Game.Online.Channels;
using Kumi.Game.Overlays.Settings.Components;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Overlays.Chat;

public partial class ChannelPickerPopover : KumiPopover
{
    private readonly FillFlowContainer<ChannelPickerItem> channelsFlow;

    public ChannelPickerPopover()
    {
        Child = new Container
        {
            Size = new Vector2(200, 600),
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                new KumiScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = channelsFlow = new FillFlowContainer<ChannelPickerItem>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y
                    }
                }
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load(ChannelManager manager)
    {
        manager.Channels.BindCollectionChanged((_, args) => Schedule(() =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    channelsFlow.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(args.NewItems != null);

                    foreach (var channel in args.NewItems)
                    {
                        if (channelsFlow.Any(i => i.Channel == channel))
                            continue;
                        
                        var item = new ChannelPickerItem((Channel) channel);
                        item.Current.Value = manager.SubscribedChannels.Contains((Channel) channel);
                        item.Current.BindValueChanged(v =>
                        {
                            if (v.NewValue)
                                manager.JoinChannel(item.Channel);

                            // TODO: Add a way to leave channels
                        });

                        channelsFlow.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    Debug.Assert(args.OldItems != null);

                    foreach (var channel in args.OldItems)
                    {
                        var item = channelsFlow.FirstOrDefault(i => i.Channel == channel);
                        if (item != null)
                            channelsFlow.Remove(item, true);
                    }

                    break;
            }
        }), true);
    }

    private partial class ChannelPickerItem : CheckboxSettingItem
    {
        public readonly Channel Channel;

        public ChannelPickerItem(Channel channel)
        {
            Channel = channel;

            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Label = channel.APIChannel.Name;
            Description = channel.APIChannel.Description;
        }
    }
}
