using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class BpmStatisticRow : ChartStatisticRow
{
    protected override Drawable CreateVisualisation() => new BpmVisualisation();

    [Resolved]
    private Bindable<APIChart?> selectedChart { get; set; } = null!;

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Label = "BPM";

        selectedChart.BindValueChanged(_ => updateValue(), true);
    }

    private void updateValue()
    {
        if (selectedChart.Value == null)
        {
            Value = "-";
            return;
        }

        var bpms = selectedChart.Value.Difficulty.BPMs;
        var commonBpm = findCommonBpm(bpms);
        var minBpm = bpms.Min();
        var maxBpm = bpms.Max();

        if (Precision.AlmostEquals(minBpm, maxBpm, 0.1f))
            Value = $"{commonBpm:0.##}";
        else
            Value = $"{commonBpm:0.##} ({minBpm:0.##} - {maxBpm:0.##})";
    }

    private float findCommonBpm(float[] bpms)
    {
        var bpm = bpms[0];
        float count = 1;

        for (var i = 1; i < bpms.Length; i++)
        {
            if (bpms[i] == bpm)
                count++;
        }

        return count / bpms.Length >= 0.5f ? bpm : 0;
    }

    private partial class BpmVisualisation : CompositeDrawable
    {
        [Resolved]
        private Bindable<APIChart?> selectedChart { get; set; } = null!;

        public BpmVisualisation()
        {
            Height = 3;
            RelativeSizeAxes = Axes.X;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        private Container barContainer = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.07f)
                },
                barContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                }
            };

            selectedChart.BindValueChanged(_ => updateBars(), true);
        }

        private void updateBars()
        {
            barContainer.Clear();

            if (selectedChart.Value == null)
                return;

            barContainer.RelativeChildSize = new Vector2(selectedChart.Value.Statistics.MusicLength, 1);

            // TODO: Store BPM time in APIChart (requires online changes)
            barContainer.AddRange(selectedChart.Value.Difficulty.BPMs.Select(bpm => new BpmBar(0)));
        }

        private partial class BpmBar : Circle
        {
            public BpmBar(float time)
            {
                X = time;
                RelativePositionAxes = Axes.X;

                RelativeSizeAxes = Axes.Y;
                Width = 3;

                Colour = Colours.YELLOW_ACCENT_LIGHT;
            }
        }
    }
}
