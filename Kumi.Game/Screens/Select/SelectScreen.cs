using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Input;
using Kumi.Game.Overlays;
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
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Select;

[Cached]
public abstract partial class SelectScreen : ScreenWithChartBackground, IKeyBindingHandler<GlobalAction>
{
    public override float BlurAmount => 10f;
    public override float DimAmount => 0.5f;

    protected abstract string FinaliseButtonText { get; }

    protected abstract Color4 FinaliseButtonColour { get; }

    [Resolved]
    private MusicController musicController { get; set; } = null!;

    [Resolved]
    protected ChartManager Manager { get; private set; } = null!;

    [Resolved]
    protected Bindable<WorkingChart> Chart { get; private set; } = null!;

    [Resolved]
    protected RealmAccess Realm { get; private set; } = null!;

    private ListSelect listSelect = null!;

    public virtual MenuItem[] CreateContextMenuItemsForChartSet(ChartSetInfo chartSetInfo)
    {
        return new[]
        {
            new MenuItem("Select", () => listSelect.SelectedChart.Value = chartSetInfo.Charts.First()),
        };
    }

    public virtual MenuItem[] CreateContextMenuItemsForChart(ChartInfo chartInfo)
    {
        return new[]
        {
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
                                new[]
                                {
                                    CreateWedge()
                                }
                            }
                        }
                    },
                }
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Padding = new MarginPadding(12),
                Child = new FillFlowContainer
                {
                    Direction = FillDirection.Horizontal,
                    AutoSizeAxes = Axes.Both,
                    Spacing = new Vector2(12, 0),
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Children = new[]
                    {
                        new KumiButton
                        {
                            RelativeSizeAxes = Axes.None,
                            Width = 150,
                            Height = 60,
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, 20),
                            Important = true,
                            BackgroundColour = FinaliseButtonColour,
                            Text = FinaliseButtonText,
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Action = () => FinaliseSelection(Chart.Value.ChartInfo)
                        },
                        new Container
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            AutoSizeAxes = Axes.Both,
                            Child = CreateExtraSelectionButtons(),
                        },
                    }
                }
            }
        };

        listSelect.SelectedChart.BindValueChanged(c =>
        {
            if (!this.IsCurrentScreen())
                return;

            workingChart.Value = Manager.GetWorkingChart(c.NewValue);
        }, true);

        Realm.Subscribe((r) => r.All<ChartSetInfo>(), (sender, _) =>
        {
            foreach (var set in sender.AsQueryable())
            {
                if (set.DeletePending)
                {
                    listSelect.RemoveChartSet(set);

                    // use random chart if the deleted chart was selected
                    if (listSelect.SelectedChart.Value?.ID == set.ID)
                        listSelect.SelectedChart.Value = set.Charts.First();
                }
                else
                {
                    if (listSelect.Groups.ContainsKey(set.ID))
                        listSelect.RemoveChartSet(set);

                    listSelect.AddChartSet(set);
                }
            }
        });

        var charts = Manager.GetAllUsableCharts();

        foreach (var set in charts)
            listSelect.AddChartSet(set);
    }

    protected virtual Drawable CreateWedge()
        => new ChartSetInfoWedge
        {
            RelativeSizeAxes = Axes.Both
        };

    protected virtual Drawable CreateExtraSelectionButtons()
        => Empty();

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
        Chart.Disabled = false;
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
                return FinaliseSelection(Chart.Value.ChartInfo);
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
    {
    }

    protected bool FinaliseSelection(ChartInfo chartInfo)
    {
        if (Chart.Disabled)
            return false;

        if (!Chart.Value.ChartInfo.Equals(chartInfo))
            Chart.Value = Manager.GetWorkingChart(chartInfo);

        Chart.Disabled = true;
        return FinaliseSelectionInternal(chartInfo);
    }

    protected abstract bool FinaliseSelectionInternal(ChartInfo chartInfo);

    private bool isHandlingLooping;

    private void beginLooping()
    {
        Debug.Assert(!isHandlingLooping);
        isHandlingLooping = true;

        ensureTrackLooping(Chart.Value);

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

        if (!Chart.Value.ChartInfo.AudioEquals(workingChart.ChartInfo))
            musicController.Play(true);
        else
            musicController.Play();
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
