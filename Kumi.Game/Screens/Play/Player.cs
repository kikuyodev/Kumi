using Kumi.Game.Charts;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Clocks;
using Kumi.Game.Gameplay.Mods;
using Kumi.Game.Gameplay.Scoring;
using Kumi.Game.Input;
using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Overlays;
using Kumi.Game.Scoring;
using Kumi.Game.Screens.Play.HUD;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;

namespace Kumi.Game.Screens.Play;

public partial class Player : ScreenWithChartBackground
{
    private const float pass_threshold = 0.7f;

    protected override OverlayActivation InitialOverlayActivation => Overlays.OverlayActivation.Disabled;
    public override float ParallaxAmount => 0.0025f;
    public override float BlurAmount => 10f;
    public override float DimAmount => 0.25f;

    public bool LoadedChartSuccessfully => playfield?.Notes.Any() == true;

    [Resolved]
    private ScoreManager scoreManager { get; set; } = null!;

    public ScoreInfo Score { get; private set; } = null!;

    private KumiPlayfield? playfield;
    private GameplayClockContainer? clockContainer;
    private GameplayKeybindContainer? keybindContainer;

    private ScoreProcessor scoreProcessor = null!;
    private HealthGaugeProcessor healthGaugeProcessor = null!;

    private HealthDisplay healthBar = null!;

    private DependencyContainer dependencies = null!;

    [Resolved]
    private IBindableList<Mod> selectedMods { get; set; } = null!;

    [Resolved]
    private IBindable<WorkingChart> chart { get; set; } = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load()
    {
        if (chart.Value is DummyWorkingChart)
            return;

        scoreProcessor = new ScoreProcessor();
        scoreProcessor.Mods = selectedMods;
        scoreProcessor.ApplyChart(chart.Value.Chart);

        healthGaugeProcessor = new HealthGaugeProcessor();
        healthGaugeProcessor.Mods = selectedMods;
        healthGaugeProcessor.ApplyChart(chart.Value.Chart);

        dependencies.CacheAs(scoreProcessor);
        dependencies.CacheAs(healthGaugeProcessor);

        InternalChild = createGameplayClockContainer(chart.Value);

        Score = CreateScore(chart.Value.Chart);

        Score.ChartInfo = chart.Value.ChartInfo;
        Score.ChartHash = chart.Value.ChartInfo.Hash;

        playfield!.NewJudgement += (_, j) =>
        {
            scoreProcessor.ApplyJudgement(j);
            healthGaugeProcessor.ApplyJudgement(j);
        };

        scoreProcessor.HasCompleted.BindValueChanged(_ => checkScoreCompleted());

        AddRangeInternal(new Drawable[]
        {
            scoreProcessor,
            healthGaugeProcessor,
            healthBar = new HealthDisplay
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.BottomRight,
                X = -24,
                Y = -110,
                Current = { BindTarget = healthGaugeProcessor.Health }
            },
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (!LoadedChartSuccessfully)
            return;

        scoreProcessor.NewJudgement += _ => scoreProcessor.PopulateScore(Score);

        foreach (var mods in selectedMods)
            mods.ApplyToPlayer(this);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        healthBar.MoveToX(0)
           .FadeOut()
           .Then()
           .MoveToX(-24, 1000, Easing.OutQuint)
           .FadeIn(1000, Easing.OutQuint);

        Scheduler.AddDelayed(() =>
        {
            if (clockContainer != null)
                clockContainer.Start();
        }, 500);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);

        chart.Value.Track.ResetSpeedAdjustments();
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

        foreach (var mod in selectedMods)
            mod.ApplyToTrack(chart.Track);

        return keybindContainer;
    }

    protected virtual Task PrepareScoreForResultsAsync(ScoreInfo score)
    {
        score.Failed = healthGaugeProcessor.Health.Value < pass_threshold;
        return Task.CompletedTask;
    }

    protected virtual ScoreInfo CreateScore(IChart chart) => new ScoreInfo
    {
        Account = new GuestAccount()
    };

    protected virtual Task ImportScore(ScoreInfo score)
    {
        var importableScore = score.DeepClone();
        var imported = scoreManager.Import(importableScore);

        score.Hash = imported.Hash;
        score.ID = imported.ID;

        return Task.CompletedTask;
    }

    private ScheduledDelegate? resultsDisplayDelegate;

    private Task<ScoreInfo?>? prepareScoreForDisplayTask;

    private void checkScoreCompleted()
    {
        if (!this.IsCurrentScreen())
            return;

        if (!scoreProcessor.HasCompleted.Value)
        {
            resultsDisplayDelegate?.Cancel();
            resultsDisplayDelegate = null;

            ValidForResume = true;
            return;
        }

        ValidForResume = false;
        progressToResults();
    }

    private void progressToResults()
    {
        resultsDisplayDelegate?.Cancel();
        resultsDisplayDelegate = new ScheduledDelegate(() =>
        {
            if (prepareScoreForDisplayTask == null)
            {
                prepareAndImportScoreAsync();
                return;
            }

            if (!prepareScoreForDisplayTask.IsCompleted)
                return;

            resultsDisplayDelegate?.Cancel();

            if (prepareScoreForDisplayTask.GetResultSafely() == null)
                return;

            if (!this.IsCurrentScreen())
                return;

            this.Push(new ResultsScreen(prepareScoreForDisplayTask.GetResultSafely()!));
        }, Time.Current + 1000, 50);

        Scheduler.Add(resultsDisplayDelegate);
    }

    private Task<ScoreInfo?> prepareAndImportScoreAsync()
    {
        if (prepareScoreForDisplayTask != null)
            return prepareScoreForDisplayTask;

        if (!scoreProcessor.HasCompleted.Value)
            return Task.FromResult<ScoreInfo?>(null);

        var scoreCopy = Score.DeepClone();
        var statisticsCopy = scoreCopy.Statistics.DeepClone();

        return prepareScoreForDisplayTask = Task.Run(async () =>
        {
            try
            {
                await PrepareScoreForResultsAsync(scoreCopy).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Score preparation failed");
            }

            try
            {
                await ImportScore(scoreCopy).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Score import failed");
            }
            
            // For some reason, the statistics aren't being automatically copied over,
            // resulting in an exception when trying to access any properties of the statistics. (RealmClosedException)
            scoreCopy.Statistics = statisticsCopy;
            return scoreCopy;
        })!;
    }
}
