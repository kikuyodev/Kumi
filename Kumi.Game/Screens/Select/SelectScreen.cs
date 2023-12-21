using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Input;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Edit;
using Kumi.Game.Screens.Play;
using Kumi.Game.Screens.Select.List;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Select;

[Cached]
public partial class SelectScreen : ScreenWithChartBackground, IKeyBindingHandler<GlobalAction>
{
    public override float BlurAmount => 10f;
    public override float DimAmount => 0.5f;

    [Resolved]
    private MusicController musicController { get; set; } = null!;

    [Resolved]
    private ChartManager manager { get; set; } = null!;
    
    [Resolved]
    private Bindable<WorkingChart> chart { get; set; } = null!;

    private ListSelect listSelect = null!;
    
    public virtual MenuItem[] CreateContextMenuItemsForChartSet(ChartSetInfo chartSetInfo)
    {
        return new[]
        {
            new MenuItem("Play", () => FinaliseSelection(chartSetInfo.Charts.First())),
            new MenuItem("Edit", () =>
            {
                chart.Value = manager.GetWorkingChart(chartSetInfo.Charts.First());
                this.Push(new Editor());
            }),
            new MenuItem("Select", () => listSelect.SelectedChart.Value = chartSetInfo.Charts.First()),
        };
    }
    
    public virtual MenuItem[] CreateContextMenuItemsForChart(ChartInfo chartInfo)
    {
        return new[]
        {
            new MenuItem("Play", () => FinaliseSelection(chartInfo)),
            new MenuItem("Edit", () =>
            {
                chart.Value = manager.GetWorkingChart(chartInfo);
                this.Push(new Editor());
            }),
            new MenuItem("Select", () => listSelect.SelectedChart.Value = chartInfo),
        };
    }

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingChart> workingChart)
    {
        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(maxSize: 840),
                    new Dimension(GridSizeMode.Absolute, 32),
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new BasicScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarVisible = true,
                            Masking = false,
                            Children = new Drawable[]
                            {
                                new BasicContextMenuContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Child = listSelect = new ListSelect
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Padding = new MarginPadding { Left = 32 },
                                        Children = new Drawable[]
                                        {
                                            new HalfScrollContainer(this) { RelativeSizeAxes = Axes.X },
                                        }
                                    }
                                }
                            }
                        },
                        Empty(), // padding
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Margin = new MarginPadding
                            {
                                Top = 32
                            },
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.Absolute, 180),
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new ChartSetInfoWedge
                                    {
                                        RelativeSizeAxes = Axes.Both
                                    }
                                }
                            }
                        }
                    },
                }
            }
        };

        listSelect.SelectedChart.BindValueChanged(c =>
        {
            if (!this.IsCurrentScreen())
                return;

            workingChart.Value = manager.GetWorkingChart(c.NewValue);
        }, true);

        var charts = manager.GetAllUsableCharts();
        charts = charts.Where(c => c.Charts.Any() && !c.DeletePending).ToList();

        Schedule(() =>
        {
            foreach (var set in charts)
                listSelect.AddChartSet(set);

            // TODO: Ideally, this should always be the last child no matter what.
            listSelect.Add(new HalfScrollContainer(this) { RelativeSizeAxes = Axes.X });
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        this.FadeInFromZero(250);
        beginLooping();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);

        // Resuming from player loader.
        chart.Disabled = false;
        beginLooping();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);

        endLooping();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        endLooping();
        listSelect.SelectedChart.UnbindAll();

        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
    {
        if (e.Repeat || !this.IsCurrentScreen())
            return false;

        switch (e.Action)
        {
            case GlobalAction.Select:
                return FinaliseSelection(chart.Value.ChartInfo);
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }
    
    protected virtual bool FinaliseSelection(ChartInfo chartInfo)
    {
        if (chart.Disabled)
            return false;

        if (!chart.Value.ChartInfo.Equals(chartInfo))
            chart.Value = manager.GetWorkingChart(chartInfo);
        
        chart.Disabled = true;
        this.Push(new PlayerLoader());
        return true;
    }

    private bool isHandlingLooping;

    private void beginLooping()
    {
        Debug.Assert(!isHandlingLooping);
        isHandlingLooping = true;

        ensureTrackLooping(chart.Value);

        musicController.TrackChanged += ensureTrackLooping;
    }

    private void endLooping()
    {
        if (!isHandlingLooping)
            return;

        musicController.CurrentTrack.Looping = isHandlingLooping = false;
        musicController.TrackChanged -= ensureTrackLooping;
    }

    private void ensureTrackLooping(IWorkingChart workingChart)
    {
        workingChart.PrepareTrackForPreview(true);
        musicController.Play(true);
    }

    private partial class HalfScrollContainer : CompositeDrawable
    {
        private readonly Drawable source;

        public HalfScrollContainer(Drawable source)
        {
            this.source = source;
        }

        protected override void Update()
        {
            base.Update();

            Height = source.DrawHeight / 2;
        }
    }
}
