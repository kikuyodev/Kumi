using Kumi.Game.Charts;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Clocks;
using Kumi.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace Kumi.Game.Screens.Play;

public partial class Player : ScreenWithChartBackground
{
    public override float BlurAmount => 0.0025f;
    public override float DimAmount => 0.25f;
    
    public bool LoadedChartSuccessfully => playfield?.Notes.Any() == true;

    private KumiPlayfield? playfield;
    private GameplayClockContainer? clockContainer;
    private GameplayKeybindContainer? keybindContainer;

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> chart)
    {
        if (chart.Value is DummyWorkingChart)
            return;

        InternalChild = createGameplayClockContainer(chart.Value);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        Scheduler.AddDelayed(() =>
        {
            if (clockContainer != null)
                clockContainer.Start();
        }, 500);
    }

    private Drawable createGameplayClockContainer(WorkingChart chart, double startDelay = 1000)
    {
        if (!chart.TrackLoaded)
            chart.LoadChartTrack();

        playfield = new KumiPlayfield(chart);
        keybindContainer = new GameplayKeybindContainer();

        clockContainer = new GameplayClockContainer(chart.Track);
        clockContainer.Reset(-startDelay);

        keybindContainer.Child = clockContainer;
        clockContainer.Child = playfield;

        playfield.Clock = clockContainer;
        playfield.ProcessCustomClock = false;

        return keybindContainer;
    }
}
