using Kumi.Game.Charts.Timings;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit.Timing;

public partial class DrawableTimingPoint : ClickableContainer
{
    private readonly TimingPoint point;
    
    [Resolved(name: "selected_point")]
    private Bindable<TimingPoint> selectedPoint { get; set; } = null!;

    public DrawableTimingPoint(TimingPoint point)
    {
        this.point = point;

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }
    
    private TimingPointConfiguration configuration = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            configuration = new TimingPointConfiguration(point),
            new TimingPointSummary(point)
            {
                Action = () => selectedPoint.Value = point
            },
        };
        
        selectedPoint.BindValueChanged(onSelectedPointChanged, true);
    }
    
    private void onSelectedPointChanged(ValueChangedEvent<TimingPoint> newPoint)
    {
        if (ReferenceEquals(newPoint.NewValue, point))
            configuration.Show();
        else
            configuration.Hide();
    }
}
