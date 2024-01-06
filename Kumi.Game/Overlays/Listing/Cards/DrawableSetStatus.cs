using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Listing.Cards;

public partial class DrawableSetStatus : CompositeDrawable
{
    private readonly APIChartSetStatus status;

    public DrawableSetStatus(APIChartSetStatus status)
    {
        this.status = status;

        AutoSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 3;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = getBackgroundColour()
            },
            new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 14),
                Colour = getForegroundColour(),
                Text = status.GetDescription().ToUpperInvariant(),
                UseFullGlyphHeight = false,
                Padding = new MarginPadding(4)
            }
        };
    }

    private Color4 getBackgroundColour()
        => status switch
        {
            APIChartSetStatus.Ranked => Color4Extensions.FromHex("39AC73"),
            APIChartSetStatus.Pending => Color4Extensions.FromHex("AC7339"),
            APIChartSetStatus.WorkInProgress => Color4Extensions.FromHex("AC4339"),
            APIChartSetStatus.Qualified => Color4Extensions.FromHex("3986AC"),
            APIChartSetStatus.Graveyard => Color4Extensions.FromHex("161B1D"),
            _ => Colours.GRAY_2
        };

    private Color4 getForegroundColour()
        => status switch
        {
            APIChartSetStatus.Graveyard => Color4Extensions.FromHex("576E75"),
            _ => Colours.GRAY_C
        };
}
