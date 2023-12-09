using Kumi.Game.Charts;
using Kumi.Game.Overlays;
using Kumi.Game.Screens;
using Kumi.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Threading;

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
        
        Chart.BindValueChanged(chartChanged, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponent(loaderScreen = new LoaderScreen());
        screenStack.Push(loaderScreen);

        DependencyContainer.CacheAs(this);

        Task.Factory.StartNew(() =>
        {
            loadComponents(new[]
            {
                new ComponentLoadTask(typeof(MusicController), loadComplete: Add),
                new ComponentLoadTask(typeof(TaskbarOverlay)),
            }, () =>
            {
                LoadComponentAsync(new MenuScreen(), c =>
                {
                    Scheduler.AddDelayed(() => screenStack.Push(c), 100); 
                });
            }, Scheduler);
        });
    }

    private void chartChanged(ValueChangedEvent<WorkingChart> chart)
    {
        chart.OldValue?.CancelAsyncLoad();
        chart.NewValue?.BeginAsyncLoad();
    }

    private void loadComponents(IReadOnlyList<ComponentLoadTask> components, Action loadComplete, Scheduler scheduler)
    {
        var amount = components.Count;

        for (var i = 0; i < amount; i++)
        {
            var component = components[i];
            LoadComponent(component.Drawable);

            component.CacheDrawable(DependencyContainer);
            var idx = i;
            scheduler.Add(() =>
            {
                component.LoadComplete?.Invoke(component.Drawable);
                loaderScreen.SetProgress((idx + 1) / (float)amount);
            });
            
            Thread.Sleep(100);
        }

        scheduler.Add(loadComplete);
    }

    private readonly struct ComponentLoadTask
    {
        public Drawable Drawable { get; }
        public Action<Drawable>? LoadComplete { get; }

        private readonly bool cache;
        private readonly Type type;

        public ComponentLoadTask(Type type, bool cache = true, Action<Drawable>? loadComplete = null)
        {
            this.cache = cache;
            this.type = type;
            LoadComplete = loadComplete;
            Drawable = (Drawable) Activator.CreateInstance(type)!;
        }

        public void CacheDrawable(DependencyContainer container)
        {
            if (!cache)
                return;

            container.CacheAs(type, Drawable);
        }
    }
}
