using System.Numerics;
using Kumi.Game.Charts;
using Kumi.Game.Screens.Edit.Timeline.Parts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineTickDisplay : TimelinePart<DrawableTick>
{
    [Resolved]
    private Bindable<WorkingChart> working { get; set; } = null!;

    [Resolved]
    private BindableBeatDivisor beatDivisor { get; set; } = null!;

    [Resolved]
    private Timeline? timeline { get; set; }

    private readonly Cached tickCache = new Cached();

    private (float min, float max) visibleRange = (float.MinValue, float.MaxValue);
    private float? nextMinTick;
    private float? nextMaxTick;
    private Chart chart = null!;

    public TimelineTickDisplay()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        beatDivisor.BindValueChanged(_ => invalidateTicks());
        working.BindValueChanged(c => chart = (Chart) c.NewValue.Chart, true);
    }

    private void invalidateTicks()
    {
        tickCache.Invalidate();
    }

    protected override void Update()
    {
        base.Update();

        if (timeline == null || DrawWidth <= 0) return;

        var newRange = (
                           (ToLocalSpace(timeline.ScreenSpaceDrawQuad.TopLeft).X - DrawableTick.TICK_MAX_WIDTH * 2) / DrawWidth * Content.RelativeChildSize.X,
                           (ToLocalSpace(timeline.ScreenSpaceDrawQuad.TopRight).X + DrawableTick.TICK_MAX_WIDTH * 2) / DrawWidth * Content.RelativeChildSize.X
                       );

        if (visibleRange != newRange)
        {
            visibleRange = newRange;

            if (nextMinTick == null || nextMaxTick == null || (visibleRange.min < nextMinTick || visibleRange.max > nextMaxTick))
                invalidateTicks();
        }

        if (!tickCache.IsValid)
            createTicks();
    }

    private void createTicks()
    {
        const float tick_height = 0.8f;
        var drawableIndex = 0;

        nextMinTick = null;
        nextMaxTick = null;

        for (int i = 0; i < chart.TimingHandler.UninheritedTimingPoints.Count; i++)
        {
            var point = chart.TimingHandler.UninheritedTimingPoints[i];
            var until = i + 1 < chart.TimingHandler.UninheritedTimingPoints.Count ? chart.TimingHandler.UninheritedTimingPoints[i + 1].Time : working.Value.Track.Length;

            var beat = 0;

            for (var t = point.Time; t < until; t += point.MillisecondsPerBeat / beatDivisor.Value)
            {
                var xPos = (float) t;

                if (t < visibleRange.min)
                    nextMinTick = xPos;
                else if (t > visibleRange.max)
                    nextMaxTick ??= xPos;
                else
                {
                    if (beat == 0 && i == 0)
                        nextMinTick = float.MinValue;

                    var indexInBar = beat % (point.TimeSignature.Numerator * beatDivisor.Value);

                    var divisor = BindableBeatDivisor.GetDivisorForBeatIndex(beat, beatDivisor.Value);
                    var colour = BindableBeatDivisor.GetColourFor(divisor);

                    var size = new Vector2(1f, tick_height);
                    
                    if (indexInBar != 0)
                        size = new Vector2(0.6f, tick_height * BindableBeatDivisor.GetHeightFor(divisor));

                    var tick = getNextDrawableTick();
                    tick.X = xPos;
                    tick.Width = DrawableTick.TICK_MAX_WIDTH * size.X;
                    tick.Height = size.Y;
                    tick.Colour = colour;
                }

                beat++;
            }
        }

        var usedDrawables = drawableIndex;

        while (drawableIndex < Math.Min(usedDrawables + 16, Count))
            Children[drawableIndex++].Hide();

        while (drawableIndex < Count)
            Children[drawableIndex++].Expire();
        
        tickCache.Validate();

        Drawable getNextDrawableTick()
        {
            DrawableTick tick;

            if (drawableIndex >= Count)
                Add(tick = new DrawableTick());
            else
                tick = Children[drawableIndex];

            drawableIndex++;
            tick.Show();

            return tick;
        }
    }
}
