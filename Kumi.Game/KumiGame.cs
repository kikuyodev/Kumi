using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Input;
using Kumi.Game.Online.Server.Packets.Dispatch;
using Kumi.Game.Overlays;
using Kumi.Game.Overlays.Control;
using Kumi.Game.Screens;
using Kumi.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osuTK.Graphics;

namespace Kumi.Game;

public partial class KumiGame : KumiGameBase, IKeyBindingHandler<GlobalAction>, IOverlayManager
{
    private bool taskbarEnabled;
    private bool taskbarControlsDisabled;

    private float taskbarOffset => (Taskbar?.Position.Y ?? 0) + (Taskbar?.DrawHeight ?? 0);

    private Container screenContainer = null!;
    private Container screenOffsetContainer = null!;
    private Container topOverlayContainer = null!;

    private KumiScreenStack screenStack = null!;
    private LoaderScreen loaderScreen = null!;

    private Container overlayContent = null!;
    private Container overlayOffsetContainer = null!;
    private Container rightOverlayContainer = null!;

    private readonly List<KumiFocusedOverlayContainer> focusedOverlays = new List<KumiFocusedOverlayContainer>();
    private readonly List<OverlayContainer> externalOverlays = new List<OverlayContainer>();
    private readonly List<OverlayContainer> visibleBlockingOverlays = new List<OverlayContainer>();

    protected TaskbarOverlay? Taskbar { get; private set; }
    protected ControlOverlay? ControlOverlay { get; private set; }
    private List<string> importTasks = new();
    private ScheduledDelegate? importTask;
    
    public KumiGame()
    {
        Name = "Kumi";
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            screenOffsetContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Child = screenContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = screenStack = new KumiScreenStack { RelativeSizeAxes = Axes.Both },
                },
            },
            overlayOffsetContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    overlayContent = new Container { RelativeSizeAxes = Axes.Both },
                    rightOverlayContainer = new Container { RelativeSizeAxes = Axes.Both }
                }
            },
            topOverlayContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
            },
        });

        DependencyContainer.CacheAs(screenStack.BackgroundStack);

        Chart.BindValueChanged(chartChanged, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponent(loaderScreen = new LoaderScreen { ExitAction = Exit });
        screenStack.Push(loaderScreen);

        screenStack.ScreenPushed += onScreenChanged;
        screenStack.ScreenExited += onScreenChanged;

        DependencyContainer.CacheAs(this);

        Task.Factory.StartNew(() =>
        {
            pushNotificationOnConnection();

            // This has to be manually added first because of some weird fuckery i can't explain at the moment
            LoadComponent(ControlOverlay = new ControlOverlay
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight
            });

            DependencyContainer.CacheAs<INotificationManager>(ControlOverlay);
            Schedule(() => rightOverlayContainer.Add(ControlOverlay));

            loadComponents(new[]
            {
                new ComponentLoadTask(typeof(MusicController), loadComplete: Add),
                new ComponentLoadTask(typeof(AccountRegistrationOverlay), loadComplete: topOverlayContainer.Add),
                new ComponentLoadTask(typeof(TaskbarOverlay), loadComplete: c =>
                {
                    Taskbar = (TaskbarOverlay) c;
                    topOverlayContainer.Add(c);
                }),
            }, () =>
            {
                LoadComponentAsync(new MenuScreen(), c =>
                {
                    Scheduler.AddDelayed(() => screenStack.Push(c), 100);
                });
            }, Scheduler);
        });
    }
    
    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        host.Window.Title = "Kumi";

        if (host.Window != null)
        {
            host.Window.DragDrop += handleDragDropPath;
        }
    }

    private void handleDragDropPath(string path)
    {
        importTasks.Add(path);
        
        if (importTask != null)
            importTask.Cancel();

        importTask = Scheduler.AddDelayed(handlePendingImports, 1000);
    }
    
    private void handlePendingImports()
    {
        lock (importTasks)
        {
            if (importTasks.Count == 0)
                return;

            Import(importTasks.ToArray());
        }
    }

    private void chartChanged(ValueChangedEvent<WorkingChart> chart)
    {
        chart.OldValue?.CancelAsyncLoad();
        chart.NewValue?.BeginAsyncLoad();
    }

    private void onScreenChanged(IScreen current, IScreen newScreen)
    {
        if (newScreen is KumiScreen newKumiScreen)
        {
            taskbarEnabled = newKumiScreen.ShowTaskbar;
            taskbarControlsDisabled = newKumiScreen.DisableTaskbarControl;

            if (newKumiScreen.ShowTaskbar)
                Taskbar?.Show();
            else
                Taskbar?.Hide();
        }
    }

    private void pushNotificationOnConnection()
    {
        var connector = API.GetServerConnector();

        if (connector == null)
            return;

        connector.RegisterDispatchHandler<NotificationDispatchPacket>("NOTIFICATION", (n) =>
        {
            ControlOverlay!.Post(new BasicNotification
            {
                Icon = FontAwesome.Solid.Signal,
                BackgroundColour = Colours.RED_ACCENT_LIGHT,
                Header = "Server",
                Message = n.Data.Message
            });
        });
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case GlobalAction.ToggleTaskbar:
                if (!taskbarEnabled || taskbarControlsDisabled)
                    return false;

                if (Taskbar?.State.Value == Visibility.Visible)
                    Taskbar?.Hide();
                else
                    Taskbar?.Show();

                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        screenOffsetContainer.Padding = new MarginPadding { Top = taskbarOffset };
        overlayOffsetContainer.Padding = new MarginPadding { Top = taskbarOffset };
    }

    #region IOverlayManager

    private void updateBlockingOverlayColour()
        => Schedule(() => screenContainer.FadeColour(visibleBlockingOverlays.Any() ? Colours.Gray(0.5f) : Color4.White, 500, Easing.OutQuint));

    public IDisposable RegisterBlockingOverlay(OverlayContainer container)
    {
        if (container.Parent != null)
            throw new ArgumentException($"Overlays must not have a parent when registered to {nameof(IOverlayManager)}", nameof(container));

        if (externalOverlays.Contains(container))
            throw new ArgumentException($"{nameof(container)} is already registered to {nameof(IOverlayManager)}", nameof(container));

        externalOverlays.Add(container);
        overlayContent.Add(container);

        if (overlayContent is KumiFocusedOverlayContainer focusedOverlayContainer)
            focusedOverlays.Add(focusedOverlayContainer);

        return new InvokeOnDisposal(() => unregisterBlockingOverlay(container));
    }

    public void ShowBlockingOverlay(OverlayContainer container)
    {
        if (!visibleBlockingOverlays.Contains(container))
            visibleBlockingOverlays.Add(container);
        updateBlockingOverlayColour();
    }

    public void HideBlockingOverlay(OverlayContainer container)
    {
        visibleBlockingOverlays.Remove(container);
        updateBlockingOverlayColour();
    }

    private void unregisterBlockingOverlay(OverlayContainer container) => Schedule(() =>
    {
        externalOverlays.Remove(container);

        if (overlayContent is KumiFocusedOverlayContainer focusedOverlayContainer)
            focusedOverlays.Remove(focusedOverlayContainer);

        container.Expire();
    });

    #endregion

    #region Component loading

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
                loaderScreen.SetProgress((idx + 1) / (float) amount);
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

    #endregion
    
    private enum ImportTaskType
    {
        KumiChart,
    }
}
