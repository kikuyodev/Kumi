using Kumi.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace Kumi.Game.Overlays.Listing.Info;

public abstract partial class ChartStatisticRow : GridContainer
{
    protected abstract Drawable CreateVisualisation();

    protected LocalisableString Label
    {
        get => label.Text;
        set => label.Text = value;
    }

    protected LocalisableString Value
    {
        get => value.Text;
        set => this.value.Text = value;
    }

    private readonly SpriteText label;
    private readonly SpriteText value;

    protected ChartStatisticRow()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        RowDimensions = new[]
        {
            new Dimension(GridSizeMode.AutoSize)
        };

        ColumnDimensions = new[]
        {
            new Dimension(),
            new Dimension()
        };

        Content = new[]
        {
            new[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        label = new SpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Font = KumiFonts.GetFont(size: 12),
                            Colour = Colours.GRAY_C
                        },
                        value = new SpriteText
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Font = KumiFonts.GetFont(size: 12),
                            Colour = Colours.GRAY_C
                        },
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = 8 },
                    Child = CreateVisualisation()
                }
            }
        };
    }
}
