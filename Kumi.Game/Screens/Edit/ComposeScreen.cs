using Kumi.Game.Charts;
using Kumi.Game.Gameplay;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;

namespace Kumi.Game.Screens.Edit;

public partial class ComposeScreen : EditorScreenWithTimeline
{
    private Container playfieldContainer = null!;
    
    [Resolved]
    private IBindable<WorkingChart> workingChart { get; set; } = null!;
    
    [Resolved]
    private EditorClock clock { get; set; } = null!;

    public ComposeScreen()
        : base(EditorScreenMode.Compose, false)
    {
    }

    private ScheduledDelegate? playfieldLoad;
    
    protected override void LoadComplete()
    {
        base.LoadComplete();

        playfieldLoad = Scheduler.AddDelayed(() =>
        {
            if (!workingChart.Value.ChartLoaded)
                return;

            playfieldContainer.Add(new InputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1,
                Child = new KumiPlayfield(workingChart.Value)
                {
                    Clock = clock,
                    ProcessCustomClock = false
                }
            });

            playfieldLoad?.Cancel();
            playfieldLoad = null;
        }, 1, true);
    }

    protected override Drawable CreateMainContent()
        => playfieldContainer = new Container { RelativeSizeAxes = Axes.Both };
}
