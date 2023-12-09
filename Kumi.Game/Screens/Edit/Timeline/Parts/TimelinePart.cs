using Kumi.Game.Charts;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class TimelinePart : TimelinePart<Drawable>
{
}

public partial class TimelinePart<T> : Container<T>
    where T : Drawable
{
    private readonly IBindable<WorkingChart> chart = new Bindable<WorkingChart>();
    
    protected readonly IBindable<Track> Track = new Bindable<Track>();

    private readonly Container<T> content;

    protected override Container<T> Content => content;

    public TimelinePart(Container<T>? content = null)
    {
        this.content = content ?? new Container<T> { RelativeSizeAxes = Axes.Both };
        
        AddInternal(this.content);

        chart.ValueChanged += _ => updateRelativeChildSize();
        Track.ValueChanged += _ => updateRelativeChildSize();
    }

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> chart, EditorClock clock)
    {
        this.chart.BindTo(chart);
        LoadChart(chart.Value.Chart);
        
        Track.BindTo(clock.Track);
    }

    private void updateRelativeChildSize()
    {
        var trackLength = chart.Value.Track.IsLoaded ? chart.Value.Track.Length : 60000;
        content.RelativeChildSize = new Vector2((float) Math.Max(1, trackLength), 1);

        if (!chart.Value.Track.IsLoaded)
            Schedule(updateRelativeChildSize);
    }

    protected virtual void LoadChart(IChart chart)
    {
        content.Clear();
    }
}
