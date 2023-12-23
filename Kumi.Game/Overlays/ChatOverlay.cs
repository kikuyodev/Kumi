using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Online;
using Kumi.Game.Online.Channels;
using Kumi.Game.Overlays.Chat;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Kumi.Game.Overlays;

public partial class ChatOverlay : KumiFocusedOverlayContainer
{
    public const float HEIGHT = 400;

    [Resolved]
    private ChannelManager channelManager { get; set; } = null!;

    private Container content = null!;

    private readonly Bindable<Channel> currentChannel = new Bindable<Channel>();

    private readonly Dictionary<Channel, DrawableChannel> channelDrawables = new Dictionary<Channel, DrawableChannel>();

    public ChatOverlay()
    {
        RelativeSizeAxes = Axes.X;
        Height = HEIGHT;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Padding = new MarginPadding(4);
    }

    private Container channelContainer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            EdgeEffect = new EdgeEffectParameters
            {
                Hollow = true,
                Radius = 8,
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0f)
            },
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f)
                },
                new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[]
                        {
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension()
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                new ChannelListing(),
                                channelContainer = new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Left = 8 }
                                }
                            }
                        }
                    }
                }
            }
        };

        currentChannel.BindTo(channelManager.CurrentChannel);
        currentChannel.BindValueChanged(c => onChannelChanged(c.NewValue), true);
    }

    private void onChannelChanged(Channel? newChannel)
    {
        if (newChannel == null)
            return;

        var drawable = getDrawableChannel(newChannel);

        // hide previous channel
        foreach (var channel in channelDrawables.Values)
            channel.Hide();

        if (!drawable.IsLoaded)
            channelContainer.Add(drawable);

        drawable.Show();
    }

    private DrawableChannel getDrawableChannel(Channel channel)
    {
        if (channelDrawables.TryGetValue(channel, out var drawable))
            return drawable;

        channelDrawables[channel] = drawable = new DrawableChannel(channel);
        return drawable;
    }

    protected override void PopIn()
    {
        this.MoveToY(0, 300, Easing.OutQuint);
        content.FadeTo(1, 300, Easing.OutQuint);
        content.FadeEdgeEffectTo(0.25f, 300, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.MoveToY(HEIGHT, 300, Easing.OutQuint);
        content.FadeTo(0, 300, Easing.OutQuint);
        content.FadeEdgeEffectTo(0, 300, Easing.OutQuint);
    }
}
