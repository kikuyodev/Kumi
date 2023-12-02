using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Screens;
using Kumi.Game.Screens.Edit;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;

namespace Kumi.Tests.Visual.Edit;

public partial class EditorScreenTestScene : KumiScreenTestScene
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
    public void TestEditorScreen()
    {
        PushScreen(new Editor());
        WaitForScreenLoad();
    }

    protected override KumiScreen CreateScreen() => new Editor();
}
