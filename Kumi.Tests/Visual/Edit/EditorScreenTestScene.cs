using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Screens.Edit;
using Kumi.Game.Tests;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;

namespace Kumi.Tests.Visual.Edit;

public partial class EditorScreenTestScene : KumiScreenTestScene
{
    private WorkingChart? workingChart;

    public override void SetupSteps()
    {
        base.SetupSteps();
        
        Schedule(() =>
        {
            Debug.Assert(ThreadSafety.IsUpdateThread);
        
            var manager = Dependencies.Get<ChartManager>();
            workingChart = manager.GetWorkingChart(manager.GetAllUsableCharts().First().Charts.First(), true);
        
            workingChart.BeginAsyncLoad();
            workingChart.LoadChartTrack();
        
            var currentChart = Dependencies.Get<Bindable<WorkingChart>>();
            currentChart.Value = workingChart;
        });
        
        AddStep("Load editor", () => LoadScreen(new Editor()));
        AddUntilStep("Wait for editor to load", () => ScreenStack.CurrentScreen is Editor);
    }
}
