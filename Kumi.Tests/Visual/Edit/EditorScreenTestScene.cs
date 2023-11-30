using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Objects.Windows;
using Kumi.Game.Screens;
using Kumi.Game.Screens.Edit;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osuTK.Graphics;

namespace Kumi.Tests.Visual.Edit;

public partial class EditorScreenTestScene : KumiScreenTestScene
{
    private TestWorkingChart? workingChart;

    [SetUp]
    public void SetupChart()
    {
        Schedule(() =>
        {
            Debug.Assert(ThreadSafety.IsUpdateThread);
    
            workingChart = TestResources.CreateWorkingChart(AudioManager, LargeTextureStore, CreateChartData);
            var currentChart = Dependencies.Get<Bindable<WorkingChart>>();
            currentChart.Value = workingChart;
        });
    }

    [Test]
    public void TestEditorScreen()
    {
        PushScreen(new EditorScreen());
        WaitForScreenLoad();
    }

    protected override KumiScreen CreateScreen() => new EditorScreen();
    
    protected virtual void CreateChartData(Chart chart)
    {
        chart.Notes.AddRange(new[]
        {
            CreateNote(500),
            CreateNote(1000),
            CreateNote(2000, NoteType.Kat),
            CreateNote(3500, NoteType.Kat),
            CreateNote(3750, NoteType.Don, true),
            CreateNote(4000, NoteType.Kat, true),
        });
    }

    protected virtual INote CreateNote(float startTime, NoteType type = NoteType.Don, bool big = false)
    {
        return new DrumHit(startTime)
        {
            Type = type,
            Flags = big ? NoteFlags.Big : NoteFlags.None,
            NoteColor = Color4.White,
            Windows = new NoteWindows()
        };
    }
}
