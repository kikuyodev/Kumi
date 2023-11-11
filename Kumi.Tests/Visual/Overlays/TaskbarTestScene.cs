using Kumi.Game.Overlays;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;

namespace Kumi.Tests.Visual.Overlays;

public partial class TaskbarTestScene : KumiTestScene
{
    private TaskbarOverlay? overlay;

    [SetUp]
    public void Setup()
    {
        Scheduler.Add(() =>
        {
            Clear();
            overlay?.Dispose();
            overlay = new TaskbarOverlay();

            Add(overlay);
        });
    }

    [Test]
    public void TaskbarTests()
    {
        AddToggleStep("toggle taskbar", v =>
        {
            overlay!.State.Value = v ? Visibility.Visible : Visibility.Hidden;
        });
    }
}
