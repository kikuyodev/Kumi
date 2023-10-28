using Kumi.Game;
using osu.Framework.Testing;

namespace Kumi.Tests.Visual;

public partial class KumiTestScene : TestScene
{
    protected override ITestSceneTestRunner CreateRunner() => new KumiTestSceneTestRunner();

    private partial class KumiTestSceneTestRunner : KumiGameBase, ITestSceneTestRunner
    {
        private TestSceneTestRunner.TestRunner runner = null!;

        protected override void LoadAsyncComplete()
        {
            base.LoadAsyncComplete();
            Add(runner = new TestSceneTestRunner.TestRunner());
        }

        public void RunTestBlocking(TestScene test)
            => runner.RunTestBlocking(test);
    }
}
