using Kumi.Game;
using Kumi.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics.Textures;
using osu.Framework.Testing;

namespace Kumi.Tests.Visual;

public partial class KumiTestScene : TestScene
{
    private DummyAPIConnection localDummyProvider;
    
    protected IAPIConnectionProvider Provider => localDummyProvider;
    protected AudioManager AudioManager => Dependencies.Get<AudioManager>();
    protected LargeTextureStore LargeTextureStore => Dependencies.Get<LargeTextureStore>();
    
    protected override ITestSceneTestRunner CreateRunner() => new KumiTestSceneTestRunner();

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        var dependencies = new DependencyContainer(parent);

        localDummyProvider = new DummyAPIConnection();
        dependencies.CacheAs<IAPIConnectionProvider>(localDummyProvider);

        LoadComponentAsync(localDummyProvider, Add);

        return dependencies;
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
