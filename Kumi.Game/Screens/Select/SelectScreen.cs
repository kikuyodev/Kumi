using System.Diagnostics;
using Kumi.Game.Charts;
using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Overlays;
using Kumi.Game.Screens.Backgrounds;
using Kumi.Game.Screens.Select.List;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Select;

public partial class SelectScreen : KumiScreen
{
    public override BackgroundScreen CreateBackground() => new ChartBackground();
    public override float BlurAmount => 10f;
    public override float DimAmount => 0.5f;

    [Resolved]
    private MusicController musicController { get; set; } = null!;

    [Resolved]
    private IBindable<WorkingChart> chart { get; set; } = null!;
    
    private ListSelect listSelect = null!;

    [BackgroundDependencyLoader]
    private void load(Bindable<WorkingChart> workingChart, ChartManager manager)
    {
        InternalChildren = new Drawable[]
        {
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new []
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
                                listSelect = new ListSelect
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Padding = new MarginPadding { Left = 32 }
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
                                new Dimension(GridSizeMode.Absolute, 272),
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
            workingChart.Value = manager.GetWorkingChart(c.NewValue);
        }, true);
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
        
        return base.OnExiting(e);
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
}
