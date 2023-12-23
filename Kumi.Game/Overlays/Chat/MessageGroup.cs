using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Sprites;
using Kumi.Game.Online.API.Accounts;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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
                        CreateUsernameFlow(),
                    }
                }
            }
        };
    }
    
    protected virtual Drawable CreateAvatar()
        => new AvatarSprite(Account);

    protected virtual Drawable CreateUsernameFlow()
    {
        var group = Account.Groups.FirstOrDefault();
        
        var container = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(4, 0),
            Children = new[]
            {
                new DrawableChatUsername(Account)
                {
                    AccentColour = group != null ? Color4Extensions.FromHex(group.Color) : Colours.GRAY_C
                }
            }
        };
        
        container.Add(new SpriteText
        {
            Text = DateTimeOffset.Now.ToLocalTime().ToString("HH:mm"),
            Font = KumiFonts.GetFont(FontFamily.Montserrat),
            Colour = Colours.GRAY_6,
        });

        return container;
    }
    
    public void AddMessage(string content)
    {
        messages.Add(new MessageItem(content));
    }
}
