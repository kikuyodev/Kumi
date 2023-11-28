using Kumi.Game.Charts;
using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Screens.Backgrounds;
using Kumi.Game.Screens.Select.List;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Select;

public partial class SelectScreen : KumiScreen
{
    public override BackgroundScreen CreateBackground() => new ChartBackground();
    public override float BlurAmount => 10f;
    public override float DimAmount => 0.25f;
    
    private ListSelect listSelect = null!;

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingChart> workingChart, ChartManager manager)
    {
        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new []
                {
                    new Dimension(maxSize: 840),
                    new Dimension(GridSizeMode.Absolute, 64),
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new BasicScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = true,
                            Children = new Drawable[]
                            {
                                listSelect = new ListSelect
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Padding = new MarginPadding { Left = 64 }
                                }
                            }
                        },
                        // padding
                        Empty(),
                        Empty()
                    },
                }
            }
        };
        
        listSelect.SelectedChart.BindValueChanged(c =>
        {
            workingChart.Value = manager.GetWorkingChart(c.NewValue);
        });
    }
}
