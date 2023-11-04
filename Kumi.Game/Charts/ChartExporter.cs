using Kumi.Game.Database;
using osu.Framework.Platform;

namespace Kumi.Game.Charts;

public class ChartExporter : ModelExporter<ChartSetInfo>
{
    protected override string Extension => ".kcs";
    
    public ChartExporter(Storage storage, RealmAccess realm)
        : base(storage, realm)
    {
    }
}
