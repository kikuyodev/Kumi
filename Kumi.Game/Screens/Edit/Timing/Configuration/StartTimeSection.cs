using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public partial class StartTimeSection : ConfigurationSection
{
    protected override LocalisableString Title => "Time";

    [Resolved(name: "selected_point")]
    private Bindable<TimingPoint?> selectedPoint { get; set; } = null!;

    private readonly Bindable<double> startTime;

    public StartTimeSection(TimingPoint point)
        : base(point)
    {
        startTime = point.StartTimeBindable.GetBoundCopy();
    }

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    private KumiTextBox startTimeBox = null!;

    protected override Drawable[] CreateContent()
    {
        var content = new Drawable[]
        {
            new GridContainer
            {
                AutoSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 90),
                    new Dimension(GridSizeMode.AutoSize)
                },
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        startTimeBox = new KumiTextBox
                        {
                            Width = 90,
                            Height = 24,
                            PlaceholderText = "Start time",
                            BackgroundColour = Colours.Gray(0.05f)
                        },
                        new KumiButton
                        {
                            RelativeSizeAxes = Axes.None,
                            Width = 120,
                            Height = 24,
                            Margin = new MarginPadding { Left = 8 },
                            Text = "Use Current Time",
                            BackgroundColour = Color4Extensions.FromHex("3F73A6"),
                            Font = KumiFonts.GetFont(size: 12),
                            Action = () => updateStartTime(editorClock.CurrentTime)
                        }
                    }
                }
            }
        };

        startTimeBox.OnCommit += (sender, isNew) =>
        {
            if (!isNew)
                return;

            if (double.TryParse(sender.Text, out double newTime))
                updateStartTime(newTime);
            else
                Point.StartTimeBindable.TriggerChange();
        };

        startTime.BindValueChanged(startTimeChanged, true);

        return content;
    }
    
    private void startTimeChanged(ValueChangedEvent<double> v)
    {
        startTimeBox.Current.Value = $"{v.NewValue:N0}";
    }

    private void updateStartTime(double newTime)
    {
        if (newTime == Point.StartTime)
            return;

        HistoryHandler?.BeginChange();

        var clone = Point.DeepClone();
        clone.StartTime = newTime;
        
        // remove the old point
        Chart.TimingPointHandler.TimingPoints.Remove(Point);
        Chart.TimingPointHandler.TimingPoints.Add(clone);
        
        Point = Chart.TimingPointHandler.TimingPoints.SingleOrDefault(p => p.StartTime == clone.StartTime)!;
        
        startTime.UnbindAll();
        startTime.BindValueChanged(startTimeChanged, true);

        HistoryHandler?.EndChange();
        
        selectedPoint.Value = Point;
    }
}
