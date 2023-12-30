using Kumi.Game.Database;

namespace Kumi.Game.Charts;

public class ChartDownloadTransmit : IModelTransmit<ChartSetInfo>
{
    public event Action? TransmitStarted;
    public event Action<ChartSetInfo?>? TransmitCompleted;

    public bool TransmissionInProgress { get; set; }

    public void StartTransmit(ChartSetInfo model)
    {
        throw new NotImplementedException();
    }

    public void WaitForTransmit(ChartSetInfo model)
    {
        throw new NotImplementedException();
    }
}
