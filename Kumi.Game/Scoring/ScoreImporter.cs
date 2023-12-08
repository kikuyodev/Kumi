using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Game.IO.Archives;
using osu.Framework.Platform;
using Realms;

namespace Kumi.Game.Scoring;

public class ScoreImporter : RealmModelImporter<ScoreInfo>
{
    public override IEnumerable<string> HandledFileExtensions => Array.Empty<string>();
    protected override string[] HashableFileTypes => Array.Empty<string>();

    public ScoreImporter(Storage storage, RealmAccess realm)
        : base(storage, realm)
    {
    }

    protected override ScoreInfo? CreateModel(ArchiveReader archive)
    {
        return new ScoreInfo
        {
            Date = DateTimeOffset.UtcNow
        };
    }

    protected override void Populate(ScoreInfo model, ArchiveReader? archive, Realm realm, CancellationToken cancellationToken = default)
    {
        if (!model.ChartInfo!.IsManaged)
            model.ChartInfo = realm.Find<ChartInfo>(model.ChartInfo!.ID);
    }
}
