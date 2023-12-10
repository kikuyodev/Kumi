using Kumi.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Textures;
using osu.Framework.Testing;

namespace Kumi.Game.Tests;

public partial class KumiTestScene : TestScene
{
    private DummyAPIConnection localDummyProvider = null!;

    protected new DependencyContainer Dependencies = null!;

    protected IAPIConnectionProvider API => localDummyProvider;
    protected AudioManager AudioManager => base.Dependencies.Get<AudioManager>();
    protected LargeTextureStore LargeTextureStore => base.Dependencies.Get<LargeTextureStore>();

    protected override ITestSceneTestRunner CreateRunner() => new KumiTestSceneTestRunner();

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        Dependencies = new DependencyContainer(parent);

        localDummyProvider = new DummyAPIConnection();
        Dependencies.CacheAs<IAPIConnectionProvider>(localDummyProvider);

        LoadComponentAsync(localDummyProvider, Add);

        return Dependencies;
    }

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
