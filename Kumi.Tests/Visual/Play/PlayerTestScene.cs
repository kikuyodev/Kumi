using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Overlays.Select;
using Kumi.Game.Screens.Play;
using Kumi.Game.Tests;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;

namespace Kumi.Tests.Visual.Play;

public partial class PlayerTestScene : KumiScreenTestScene
{
    private WorkingChart? workingChart;

    public override void SetupSteps()
    {
        base.SetupSteps();
        
        Schedule(() =>
        {
            Debug.Assert(ThreadSafety.IsUpdateThread);
            LoadComponent(new ModSelectionOverlay());
            Dependencies.CacheAs(new ModSelectionOverlay().SelectedMods);
            
            var manager = Dependencies.Get<ChartManager>();
            workingChart = manager.GetWorkingChart(manager.GetAllUsableCharts().First().Charts.First(), true);
        
            workingChart.BeginAsyncLoad();
            workingChart.LoadChartTrack();
        
            var currentChart = Dependencies.Get<Bindable<WorkingChart>>();
            currentChart.Value = workingChart;
        });
        
        AddStep("Load editor", () => LoadScreen(new PlayerLoader()));
        AddUntilStep("Wait for editor to load", () => ScreenStack.CurrentScreen is PlayerLoader);
    }
}
