using Kumi.Game.Charts;
using Kumi.Game.Gameplay;
using Kumi.Game.Gameplay.Clocks;
using Kumi.Game.Gameplay.Scoring;
using Kumi.Game.Input;
using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Scoring;
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
    public override bool ShowTaskbar => false;
    public override bool DisableTaskbarControl => true;
    
    private const float pass_threshold = 0.7f;
    
    public override float ParallaxAmount => 0.0025f;
    public override float BlurAmount => 10f;
    public override float DimAmount => 0.25f;
    
    public bool LoadedChartSuccessfully => playfield?.Notes.Any() == true;

    [Resolved]
    private ScoreManager scoreManager { get; set; } = null!;

    public ScoreInfo Score { get; private set; }
    
    private KumiPlayfield? playfield;
    private GameplayClockContainer? clockContainer;
    private GameplayKeybindContainer? keybindContainer;

    private ScoreProcessor scoreProcessor = null!;
    private HealthGaugeProcessor healthGaugeProcessor = null!;

    private DependencyContainer dependencies = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> chart)
    {
        if (chart.Value is DummyWorkingChart)
            return;

        scoreProcessor = new ScoreProcessor();
        scoreProcessor.ApplyChart(chart.Value.Chart);
        
        healthGaugeProcessor = new HealthGaugeProcessor();
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
        };
        
        scoreProcessor.HasCompleted.BindValueChanged(_ => checkScoreCompleted());
        
        AddRangeInternal(new Drawable[]
        {
            scoreProcessor
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        if (!LoadedChartSuccessfully)
            return;

        scoreProcessor.NewJudgement += _ => scoreProcessor.PopulateScore(Score);
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
    
    protected virtual Task PrepareScoreForResultsAsync(ScoreInfo score)
        => Task.CompletedTask;
    
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

            return scoreCopy;
        })!;
    }
}
