using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class SkewedChartStatus : CompositeDrawable
{
    public SkewedChartStatus()
    {
        AutoSizeAxes = Axes.Both;
    }
    
    [Resolved]
    private Bindable<APIChartSet> selectedChartSet { get; set; } = null!;

    private SpriteText statusText = null!;
    private Box background = null!;
    
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(10 * MathF.PI / 180, 0),
                Padding = new MarginPadding { Right = -200 },
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 5,
                        Children = new Drawable[]
                        {
                            background = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colours.GRAY_2
                            }
                        }
                    }
                }
            },
            statusText = new SpriteText
            {
                Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Colours.GRAY_C,
                Padding = new MarginPadding(8)
            },
        };
        
        selectedChartSet.BindValueChanged(v =>
        {
            if (v.NewValue == null)
                return;

            statusText.Text = v.NewValue.Status.GetDescription();
            statusText.Colour = getForegroundColour(v.NewValue.Status);
            background.Colour = getBackgroundColour(v.NewValue.Status);
        }, true);
    }

    private Color4 getBackgroundColour(APIChartSetStatus status)
        => status switch
        {
            APIChartSetStatus.Ranked => Color4Extensions.FromHex("39AC73"),
            APIChartSetStatus.Pending => Color4Extensions.FromHex("AC7339"),
            APIChartSetStatus.WorkInProgress => Color4Extensions.FromHex("AC4339"),
            APIChartSetStatus.Qualified => Color4Extensions.FromHex("3986AC"),
            APIChartSetStatus.Graveyard => Color4Extensions.FromHex("161B1D"),
            _ => Colours.GRAY_2
        };

    private Color4 getForegroundColour(APIChartSetStatus status)
        => status switch
        {
            APIChartSetStatus.Graveyard => Color4Extensions.FromHex("576E75"),
            _ => Colours.GRAY_C
        };
}
