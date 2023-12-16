using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using osu.Framework.Utils;

namespace Kumi.Game.Gameplay;

public class BarLineGenerator
{
    public readonly List<BarLine> BarLines = new List<BarLine>();

    public BarLineGenerator(Chart chart)
    {
        if (chart.Notes.Count == 0)
            return;

        var lastTime = chart.Notes.Last().GetEndTime();
        var points = chart.TimingHandler.UninheritedTimingPoints;

        if (points.Count == 0)
            return;

        for (var i = 0; i < points.Count; i++)
        {
            var currentPoint = points[i];
            var currentBeat = 0;

            var endTime = i < points.Count - 1 ? points[i + 1].StartTime : lastTime + currentPoint.MillisecondsPerBeat * currentPoint.TimeSignature.Numerator;
            var barLength = currentPoint.MillisecondsPerBeat * currentPoint.TimeSignature.Numerator;

            var startTime = currentPoint.StartTime;

            for (var t = startTime; Precision.AlmostBigger(endTime, t); t += barLength, currentBeat++)
            {
                var roundedTime = Math.Round(t, MidpointRounding.AwayFromZero);

                if (Precision.AlmostEquals(t, roundedTime))
                    t = roundedTime;

                BarLines.Add(new BarLine((float) t)
                {
                    Major = { Value = currentBeat % currentPoint.TimeSignature.Numerator == 0 }
                });
            }
        }
    }
}
