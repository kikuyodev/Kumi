using Kumi.Game.Overlays;
using Kumi.Game.Screens;
using Kumi.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Kumi.Game;

public partial class KumiGame : KumiGameBase
{
    private KumiScreenStack screenStack = null!;
    private LoaderScreen loaderScreen = null!;

    protected MusicController MusicController { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            screenStack = new KumiScreenStack { RelativeSizeAxes = Axes.Both }
        });

        DependencyContainer.CacheAs(screenStack.BackgroundStack);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponent(loaderScreen = new LoaderScreen());
        screenStack.Push(loaderScreen);

        DependencyContainer.CacheAs(this);

        loadComponents(new[]
        {
            new ComponentLoadTask<MusicController>(loadComplete: Add)
        }, () =>
        {
            LoadComponentAsync(new MenuScreen(), screenStack.Push);
        });
    }

    private void loadComponents<T>(IReadOnlyList<ComponentLoadTask<T>> components, Action loadComplete)
        where T : Drawable, new()
    {
        var amount = components.Count;

        for (var i = 0; i < amount; i++)
        {
            var component = components[i];
            LoadComponentAsync(component.Drawable, c =>
            {
                component.CacheDrawable(DependencyContainer);
                component.LoadComplete?.Invoke(c);
            }).Wait();
            loaderScreen.SetProgress((float) i / amount);
        }

        loadComplete();
    }

    private struct ComponentLoadTask<T>
        where T : Drawable, new()
    {
        public T Drawable { get; }
        public Action<Drawable>? LoadComplete { get; }

        private readonly bool cache;

        public ComponentLoadTask(bool cache = true, Action<Drawable>? loadComplete = null)
        {
            this.cache = cache;
            LoadComplete = loadComplete;
            Drawable = new T();
        }

        public ComponentLoadTask(T drawable, bool cache = true, Action<Drawable>? loadComplete = null)
        {
            this.cache = cache;
            LoadComplete = loadComplete;
            Drawable = drawable;
        }

        public void CacheDrawable(DependencyContainer container)
        {
            if (!cache)
                return;

            container.CacheAs(Drawable);
        }
    }
}
