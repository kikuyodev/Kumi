using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Screens.Play;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;

namespace Kumi.Tests.Visual.Play;

public partial class PlayerTestScene : KumiScreenTestScene
{
    private WorkingChart? workingChart;

    [SetUp]
    public void SetupChart()
    {
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
    }
    
    [Test]
    public void TestPlayerScreen()
    {
        PushScreen(new PlayerLoader());
        WaitForScreenLoad();
    }
}
