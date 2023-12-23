using Kumi.Game.Graphics;
using Kumi.Game.Online.API;
using Kumi.Game.Online.Server;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Overlays.Control;

public partial class ConnectionSection : Container
{
    private SpriteText statusText = null!;
    private RollingSpriteText pingText = null!;

    private readonly IBindable<ServerConnectionState> state = new Bindable<ServerConnectionState>(ServerConnectionState.Disconnected);

    private IServerConnector connector = null!;

    public ConnectionSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(8);
    }

    [BackgroundDependencyLoader]
    private void load(IAPIConnectionProvider api)
    {
        Children = new Drawable[]
        {
            new SpriteText
            {
                Text = "Connection",
                Font = KumiFonts.GetFont(size: 12),
                Colour = Colours.GRAY_C
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                AutoSizeDuration = 200,
                AutoSizeEasing = Easing.OutQuint,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4),
                Children = new Drawable[]
                {
                    statusText = new SpriteText
                    {
                        Font = KumiFonts.GetFont(weight: FontWeight.SemiBold, size: 12),
                        Colour = Colours.GRAY_6,
                        Text = "DISCONNECTED",
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                    },
                    pingText = new RollingSpriteText
                    {
                        Font = KumiFonts.GetFont(size: 12),
                        Colour = Colours.GRAY_6,
                        Text = "0ms",
                        Alpha = 0,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                    }
                }
            }
        };

        connector = api.GetServerConnector()!;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        state.BindTo(connector.State);
        state.BindValueChanged(_ => onStateChanged(), true);
    }

    private ScheduledDelegate? pingScheduler;

    private void onStateChanged()
    {
        switch (state.Value)
        {
            default:
            case ServerConnectionState.Disconnected:
                statusText.Text = "DISCONNECTED";
                statusText.Colour = Colours.GRAY_6;
                pingText.Alpha = 0;

                pingScheduler?.Cancel();
                pingScheduler = null;
                break;

            case ServerConnectionState.Connecting:
                statusText.Text = "CONNECTING";
                statusText.Colour = Colours.YELLOW;
                pingText.Alpha = 0;
                break;

            case ServerConnectionState.Connected:
                statusText.Text = "CONNECTED";
                statusText.Colour = Colours.BLUE;
                pingText.Alpha = 1;

                startPing();
                break;
        }
    }

    private void startPing()
    {
        pingScheduler?.Cancel();
        pingScheduler = null;

        Scheduler.Add(pingScheduler = new ScheduledDelegate(async () =>
        {
            if (!IsPresent)
                return;
            
            var ping = await connector.CurrentConnection!.Ping();

            pingText.Delay.Value = (int) ping;
            pingScheduler = null;
        }, 0, 1000));
    }
    
    private partial class RollingSpriteText : SpriteText
    {
        public Bindable<int> Delay { get; set; } = new Bindable<int>();

        private int displayCount;
        
        public int DisplayCount
        {
            get => displayCount;
            set
            {
                displayCount = value;
                Text = $"{displayCount}ms";
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            Delay.BindValueChanged(_ => transformCount(), true);
        }
        
        private void transformCount()
        {
            this.TransformTo(nameof(DisplayCount), Delay.Value, 750, Easing.OutQuint);
        }
    }
}
