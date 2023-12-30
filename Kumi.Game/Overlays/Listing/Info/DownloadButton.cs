using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API.Charts;
using Kumi.Game.Online.API.Requests;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Info;

// TODO: Figure out if the chart set is already downloaded
public partial class DownloadButton : KumiProgressButton
{
    [Resolved]
    private Bindable<APIChartSet> selectedChartSet { get; set; } = null!;

    [Resolved]
    private ChartDownloadTransmit downloadTransmit { get; set; } = null!;
    
    public DownloadButton()
    {
        Label = "Download";
        Icon = FontAwesome.Solid.Download;
        IconScale = new Vector2(0.6f);
        BackgroundColour = Colours.CYAN_LIGHT;

        Action = () =>
        {
            if (downloadTransmit.TransmissionInProgress)
                return;
            
            tryDownload();
        };
    }

    private void tryDownload()
    {
        downloadTransmit.ModifyRequest += modifyRequest;
        downloadTransmit.TransmitStarted += onTransmissionStarted;
        downloadTransmit.TransmitCompleted += onTransmissionCompleted;

        var info = new ChartSetInfo { OnlineID = selectedChartSet.Value.Id };
        downloadTransmit.StartTransmit(info);
        downloadTransmit.WaitForTransmit(info);
    }

    private void modifyRequest(DownloadChartSetRequest req)
    {
        req.DownloadProgress += (current, length) => Progress = (float) current / length;
    }

    private void onTransmissionStarted()
    {
        State = ButtonState.Loading;
        selectedChartSet.Disabled = true;
    }

    private void onTransmissionCompleted(ChartSetInfo? model)
    {
        State = ButtonState.Idle;
        selectedChartSet.Disabled = false;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        
        downloadTransmit.ModifyRequest -= modifyRequest;
        downloadTransmit.TransmitStarted -= onTransmissionStarted;
        downloadTransmit.TransmitCompleted -= onTransmissionCompleted;
    }
}
