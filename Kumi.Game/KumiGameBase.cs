using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Game.Graphics;
using Kumi.Game.Input;
using Kumi.Game.Screens;
using Kumi.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace Kumi.Game;

public partial class KumiGameBase : osu.Framework.Game
{
    protected Storage Storage { get; set; } = null!;

    private RealmAccess realm = null!;
    private ChartManager chartManager = null!;
    private KeybindStore keybindStore = null!;
    
    protected KumiScreenStack ScreenStack = null!;
    protected Colors GameColors { get; private set; } = null!;
    protected override Container<Drawable> Content { get; }

    protected DependencyContainer DependencyContainer = null!;
    
    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        DependencyContainer = new DependencyContainer(base.CreateChildDependencies(parent));
        return DependencyContainer;
    }

    protected KumiGameBase()
    {
        base.Content.Add(Content = new DrawSizePreservingFillContainer
        {
            TargetDrawSize = new Vector2(1280, 720)
        });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        // Resources
        Resources.AddStore(new DllResourceStore(KumiResources.Assembly));
        DependencyContainer.Cache(GameColors = new Colors());
        
        // Realm Database & Storage
        DependencyContainer.Cache(realm = new RealmAccess(Storage));
        DependencyContainer.CacheAs(Storage);
        var defaultChart = new DummyWorkingChart(Audio, Textures);
        DependencyContainer.Cache(chartManager = new ChartManager(Storage, realm, Audio, Resources, Host, defaultChart));

        var largeStore = new LargeTextureStore(Host.Renderer, Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, "Textures")));
        largeStore.AddTextureSource(Host.CreateTextureLoaderStore(new OnlineStore()));
        DependencyContainer.Cache(largeStore);

        GlobalKeybindContainer globalKeybindContainer;
        
        base.Content.Add(new SafeAreaContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = new DrawSizePreservingFillContainer() // TODO: Add a way to change the resolution and UI scale dynamically.
            {
                TargetDrawSize = new Vector2(1920, 1080),
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    globalKeybindContainer = new GlobalKeybindContainer()
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                }
            }
        });
        
        DependencyContainer.Cache(globalKeybindContainer);
        DependencyContainer.Cache(keybindStore = new KeybindStore(realm));
        keybindStore.AssignDefaultsFor(globalKeybindContainer);
        keybindStore.RegisterDefaults();
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        Storage ??= host.Storage;
    }
}
