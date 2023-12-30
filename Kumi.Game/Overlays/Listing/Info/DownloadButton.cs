using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Info;

// TODO: Figure out if the chart set is already downloaded
public partial class DownloadButton : KumiProgressButton
{
    [Resolved]
    private Bindable<APIChartSet> selectedChartSet { get; set; } = null!;

    private bool downloadInProgress;

    public DownloadButton()
    {
        Label = "Download";
        Icon = FontAwesome.Solid.Download;
        IconScale = new Vector2(0.6f);
        BackgroundColour = Colours.CYAN_LIGHT;

        Action = () =>
        {
            if (downloadInProgress)
                return;

            startDownload();
            waitForDownload();
        };
    }

    private Task? downloadTask;

    private void startDownload()
    {
        downloadInProgress = true;
        State = ButtonState.Loading;
        Enabled.Value = false;
        selectedChartSet.Disabled = true;

        downloadTask = Task.Factory.StartNew(download, TaskCreationOptions.LongRunning);
    }

    private ScheduledDelegate? waitForDownloadDelegate;

    private void waitForDownload()
    {
        Scheduler.Add(waitForDownloadDelegate = new ScheduledDelegate(() =>
        {
            if (downloadTask is not { IsCompleted: true })
                return;

            downloadTask = null;
            waitForDownloadDelegate?.Cancel();
            waitForDownloadDelegate = null;

            finishDownload();
        }, 0, 10));
    }

    private void download()
    {
        
    }

    private void finishDownload()
    {
        downloadInProgress = false;
        State = ButtonState.Idle;
        Enabled.Value = true;
        selectedChartSet.Disabled = true;
    }
}
