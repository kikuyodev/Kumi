using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Overlays.Listing.Cards;

public partial class DrawableDifficultyBars : FillFlowContainer
{
    private readonly APIChartSet chartSet;
    
    public DrawableDifficultyBars(APIChartSet chartSet)
    {
        this.chartSet = chartSet;

        Height = 12;
        AutoSizeAxes = Axes.X;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(chartSet.Charts.Select(c => new DifficultyBar(c.Difficulty.Difficulty)));
    }
    
    private partial class DifficultyBar : Circle
    {
        public DifficultyBar(float difficulty)
        {
            RelativeSizeAxes = Axes.Y;
            Width = 3;
            
            Colour = getColourFor(difficulty);
        }

        private Colour4 getColourFor(float difficulty)
            => Colours.GRAY_C;
    }
}
