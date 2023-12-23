using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online;
using Kumi.Game.Online.Channels;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Chat;

public partial class DrawableChannelItem : CompositeDrawable
{
    private string icon = string.Empty;

    public string Icon
    {
        get => icon;
        set
        {
            icon = value;
            iconText.Text = icon;
        }
    }

    private LocalisableString channelName;

    public LocalisableString ChannelName
    {
        get => channelName;
        set
        {
            channelName = value;
            nameText.Text = channelName;
        }
    }

    public Action<Channel?>? Action;
    public Action<Channel?>? OnClose;

    private readonly Box background;
    private readonly SpriteText iconText;
    private readonly SpriteText nameText;

    public readonly Channel? Channel;

    public DrawableChannelItem(Channel? channel, bool showClose = true)
    {
        Channel = channel;

        if (channel != null)
            channelName = channel.APIChannel.Name;

        RelativeSizeAxes = Axes.X;
        Height = 32;
        Masking = true;
        CornerRadius = 5;

        InternalChildren = new[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.1f).Opacity(0f)
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(8, 0),
                Padding = new MarginPadding
                {
                    Left = 16
                },
                Children = new Drawable[]
                {
                    iconText = new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 16),
                        Colour = Colours.GRAY_6,
                        Text = icon!
                    },
                    nameText = new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, size: 16),
                        Colour = Colours.GRAY_6,
                        Text = channelName
                    }
                }
            },
            showClose
                ? new KumiIconButton
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Icon = FontAwesome.Solid.TimesCircle,
                    IconScale = new Vector2(0.6f),
                    Size = new Vector2(24),
                    Colour = Colours.GRAY_6,
                    Action = () => OnClose?.Invoke(channel),
                }
                : Empty()
        };
    }

    private Bindable<Channel> currentChannel = null!;

    [BackgroundDependencyLoader]
    private void load(ChannelManager channels)
    {
        currentChannel = channels.CurrentChannel.GetBoundCopy();

        currentChannel.BindValueChanged(_ => updateBackground(), true);
    }

    private void updateBackground()
    {
        if (Channel != null && currentChannel.Value.APIChannel.Id == Channel.APIChannel.Id)
        {
            background.FadeTo(1f, 100);
        }
        else
        {
            if (IsHovered)
                background.FadeTo(1f, 100);
            else
                background.FadeTo(0f, 100);
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke(Channel);
        return base.OnClick(e);
    }
}
