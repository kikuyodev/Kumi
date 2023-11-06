using Kumi.Game.Charts;
using Kumi.Game.Database;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace Kumi.Game.Overlays;

public partial class MusicController : CompositeComponent
{
    [Resolved]
    private ChartManager charts { get; set; } = null!;

    [Resolved]
    private IBindable<WorkingChart> chart { get; set; } = null!;

    [Resolved]
    private RealmAccess realm { get; set; } = null!;

    public event Action<WorkingChart>? TrackChanged; 

    public DrawableTrack CurrentTrack { get; private set; } = new DrawableTrack(new TrackVirtual(1000));

    public bool IsPlaying => CurrentTrack.IsRunning;

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        chart.BindValueChanged(c => changeChart(c.NewValue), true);
    }

    public void EnsurePlayingSong()
    {
        if (CurrentTrack.IsDummyDevice)
        {
            if (chart.Disabled)
                return;
            
            Logger.Log($"{nameof(MusicController)} skipping dummy track.");
            Next();
        } else if (!IsPlaying)
        {
            Logger.Log($"{nameof(MusicController)} starting playback to ensure something is playing.");
            Play();
        }
    }

    public void Next()
        => Schedule(() =>
        {
            if (chart.Disabled)
                return;

            var playableSet = getChartSets().AsEnumerable().SkipWhile(i => !i.Equals(current.ChartSetInfo)).ElementAtOrDefault(1)
                              ?? getChartSets().FirstOrDefault();
            var playableChart = playableSet?.Charts.FirstOrDefault();

            if (playableChart == null)
                return;

            changeChart(charts.GetWorkingChart(playableChart));
            restartTrack();
        });

    public bool Play(bool restart = false)
    {
        if (restart)
            CurrentTrack.RestartAsync();
        else if (!IsPlaying)
            CurrentTrack.StartAsync();

        return true;
    }

    public void Stop()
    {
        if (CurrentTrack.IsRunning)
            CurrentTrack.StopAsync();
    }

    public bool TogglePause()
    {
        if (CurrentTrack.IsRunning)
            Stop();
        else
            Play();

        return true;
    }

    private IQueryable<ChartSetInfo> getChartSets()
        => realm.Realm.All<ChartSetInfo>().Where(s => !s.DeletePending);

    private void restartTrack()
    {
        CurrentTrack.RestartAsync();
    }

    private WorkingChart current = null!;
    
    private void changeChart(WorkingChart newChart)
    {
        if (newChart == current)
            return;

        var last = current;
        current = newChart;
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (last == null || !last.TryTransferTrack(current))
            changeTrack();
        
        TrackChanged?.Invoke(current);

        if (chart.Value != current && chart is Bindable<WorkingChart> working)
            working.Value = current;
    }

    private void changeTrack()
    {
        var newTrack = new DrawableTrack(current.LoadChartTrack());

        var lastTrack = CurrentTrack;
        CurrentTrack = newTrack;

        Schedule(() =>
        {
            lastTrack.VolumeTo(0, 500, Easing.Out).Expire();

            if (newTrack == CurrentTrack)
            {
                AddInternal(newTrack);
                newTrack.VolumeTo(0).Then().VolumeTo(1, 500, Easing.Out);
            }
            else
            {
                newTrack.Dispose();
            }
        });
    }
}
