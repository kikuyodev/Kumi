using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Sprites;
using Kumi.Game.Online.API.Accounts;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Overlays.Chat;

public partial class MessageGroup : CompositeDrawable
{
    public readonly APIAccount Account;
    private readonly FillFlowContainer messages;
    
    public MessageGroup(APIAccount account)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Account = account;
        
        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(4, 0),
            Children = new Drawable[]
            {
                new Container
                {
                    Size = new Vector2(40),
                    Masking = true,
                    CornerRadius = 5,
                    Child = CreateAvatar()
                },
                messages = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(4),
                    Children = new[]
                    {
                        CreateUsername(),
                    }
                }
            }
        };
    }
    
    protected virtual Drawable CreateAvatar()
        => new AvatarSprite(Account);

    protected virtual Drawable CreateUsername()
    {
        var container = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(4, 0),
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = Account.Username,
                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                    Colour = Colours.GRAY_C
                },
            }
        };

        if (Account.Groups.Any())
        {
            var group = Account.Groups.First();
            
            container.Add(new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(4, 0),
                Children = new Drawable[]
                {
                    new Circle
                    {
                        Width = 3,
                        RelativeSizeAxes = Axes.Y,
                        Colour = Color4Extensions.FromHex(group.Color)
                    },
                    new SpriteText
                    {
                        Text = group.Tag,
                        Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                        Colour = Color4Extensions.FromHex(group.Color)
                    }
                }
            });
        }
        
        container.Add(new SpriteText
        {
            Text = DateTimeOffset.Now.ToLocalTime().ToString("HH:mm"),
            Font = KumiFonts.GetFont(FontFamily.Montserrat),
            Colour = Colours.GRAY_6,
        });

        return container;
    }
    
    public void AddMessage(string message)
    {
        messages.Add(new MessageItem(message));
    }
}
