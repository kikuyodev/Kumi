using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Sprites;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Accounts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Overlays.Taskbar;

public partial class TaskbarUserButton : TaskbarButton
{
    private SpriteText username = null!;
    private UpdateableAvatarSprite avatar = null!;

    [BackgroundDependencyLoader(true)]
    private void load(IAPIConnectionProvider? connection)
    {
        connection?.LocalAccount.BindValueChanged(a =>
        {
            username.Text = a.NewValue?.Username ?? "Guest";
            avatar.Account = a.NewValue ?? new GuestAccount();
        }, true);
    }

    protected override Drawable CreateContent()
        => new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            RelativeSizeAxes = Axes.Y,
            AutoSizeAxes = Axes.X,
            Spacing = new Vector2(8f, 0),
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Child = username = new SpriteText
                    {
                        Text = "Guest",
                        Font = KumiFonts.GetFont(size: 14),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Size = new Vector2(32),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Masking = true,
                            CornerRadius = 5,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colours.Gray(0.1f),
                            }
                        },
                        avatar = new UpdateableAvatarSprite
                        {
                            Size = new Vector2(32),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                    }
                },
            }
        };
}
