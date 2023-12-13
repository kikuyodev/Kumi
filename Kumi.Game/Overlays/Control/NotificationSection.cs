using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Overlays.Control;

public partial class NotificationSection : FillFlowContainer
{
    public IEnumerable<Notification> Notifications => notifications;

    public int DisplayedCount => notifications.Count(n => !n.IsClosed);
    public int UnreadCount => notifications.Count(n => !n.IsClosed && !n.Read);
    
    private FlowContainer<Notification> notifications = null!;
    
    private SpriteText countText = null!;

    public NotificationSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 8);
        Padding = new MarginPadding(8);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = "Notifications",
                        Font = KumiFonts.GetFont(),
                        Colour = Colours.GRAY_C
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(8, 0),
                        Children = new Drawable[]
                        {
                            new ClearAllButton
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Text = "Clear All",
                                Action = clearAll
                            },
                            countText = new SpriteText
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Font = KumiFonts.GetFont(size: 12),
                                Colour = Colours.GRAY_6
                            }
                        }
                    }
                }
            },
            notifications = new FillFlowContainer<Notification>
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 4)
            }
        };
    }

    public void Add(Notification notification)
    {
        notifications.Add(notification);
    }

    public void MarkAllRead()
    {
        notifications.Children.ForEach(c => c.Read = true);
    }

    protected override void Update()
    {
        base.Update();
        
        countText.Text = $"({getCount().ToString()})";
    }

    private void clearAll()
        => notifications.Children.ForEach(c => c.Close());

    private int getCount()
        => notifications.Count(c => c.Alpha > 0.99f);
    
    private partial class ClearAllButton : ClickableContainer
    {
        public LocalisableString Text
        {
            get => text.Text;
            set => text.Text = value;
        }
        
        private readonly SpriteText text;

        public ClearAllButton()
        {
            AutoSizeAxes = Axes.Both;

            Children = new[]
            {
                text = new SpriteText
                {
                    Font = KumiFonts.GetFont(size: 12),
                    Colour = Colours.GRAY_C
                }
            };
        }
    }
}
