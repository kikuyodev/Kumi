using Kumi.Game.Charts;
using Kumi.Game.Overlays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osuTK.Graphics;

namespace Kumi.Game.Screens.Play;

public partial class PlayerLoader : ScreenWithChartBackground
{
    protected override OverlayActivation InitialOverlayActivation => Overlays.OverlayActivation.UserTriggered;
    public override float BlurAmount => 0f;
    public override float DimAmount => 0.9f;

    private bool readyForPush
        => player?.LoadState == LoadState.Ready;

    private Task? loadTask;

    private Player? player;

    private ScheduledDelegate? scheduledPushPlayer;

    private Container content = null!;
    private ChartLoaderDisplay display = null!;
    private Box background = null!;

    [Resolved]
    private IBindable<WorkingChart> chart { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(Color4.Black.Opacity(0.75f), Color4.Black.Opacity(0f)),
                    },
                    display = new ChartLoaderDisplay(chart.Value.ChartInfo)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);

        contentIn();

        chart.Value.Track.Stop();

        display.Delay(700).FadeIn(500, Easing.OutQuint);

        Scheduler.Add(new ScheduledDelegate(pushWhenLoaded, Clock.CurrentTime + 1000, 0));
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);

        player = null;
        cancelLoad();

        contentIn();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        cancelLoad();
        contentOut();

        this.Delay(500).FadeOut();

        return base.OnExiting(e);
    }

    private void pushWhenLoaded()
    {
        if (!this.IsCurrentScreen())
            return;

        if (!readyForPush)
        {
            cancelLoad();
            return;
        }

        if (scheduledPushPlayer != null)
            return;

        scheduledPushPlayer = Scheduler.AddDelayed(() =>
        {
            if (!this.IsCurrentScreen())
                return;

            loadTask = null;
            ValidForResume = false;

            if (player?.LoadedChartSuccessfully ?? false)
                this.Push(player);
            else
                this.Exit();
        }, 500);
    }

    private void prepareScreen()
    {
        if (!this.IsCurrentScreen())
            return;

        player = new Player();

        loadTask = LoadComponentAsync(player, _ =>
        {
            display.IsReady.Value = true;
            background.FadeTo(0.75f, 250, Easing.OutQuint);
            CurrentBackground.BackgroundStack.DimAmount = 0.4f;
        });
    }

    private void contentIn()
    {
        content.FadeInFromZero(500, Easing.OutQuint).Then().Schedule(prepareScreen);
    }

    private void contentOut()
    {
        content.FadeOut(500, Easing.OutQuint);
    }

    private void cancelLoad()
    {
        scheduledPushPlayer?.Cancel();
        scheduledPushPlayer = null;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (isDisposing)
            loadTask?.ContinueWith(_ => player?.Dispose());
    }
}
