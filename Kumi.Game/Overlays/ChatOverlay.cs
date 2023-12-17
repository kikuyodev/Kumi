using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Overlays.Chat;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace Kumi.Game.Overlays;

public partial class ChatOverlay : KumiFocusedOverlayContainer
{
    public const float HEIGHT = 400;

    private Container content = null!;

    public ChatOverlay()
    {
        RelativeSizeAxes = Axes.X;
        Height = HEIGHT;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Padding = new MarginPadding(4);
    }

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
                new GridContainer
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
                            new DrawableChannel()
                        }
                    }
                }
            }
        };
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
