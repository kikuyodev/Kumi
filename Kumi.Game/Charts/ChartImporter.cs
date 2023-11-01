using Kumi.Game.Database;
using Kumi.Game.IO.Archives;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace Kumi.Game.Charts;

public class ChartImporter : RealmModelImporter<ChartSetInfo>
{
    private const string kumi_archive = ".kar";
    private const string kumi_chart = ".kch";
    
    public override IEnumerable<string> HandledFileExtensions => new[] { kumi_archive };

    protected override string[] HashableFileTypes => new[] { kumi_chart };

    public Action<ChartSetInfo>? ProcessChart { private get; set; }

    public ChartImporter(Storage storage, RealmAccess realm)
        : base(storage, realm)
    {
    }

    protected override ChartSetInfo? CreateModel(ArchiveReader archive)
    {
        string? chartsExists = archive.FileNames.FirstOrDefault(f => f.EndsWith(kumi_chart, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(chartsExists))
        {
            Logger.Log($"No chart files found in the chart archive ({archive.ArchivePath})", LoggingTarget.Database);
            return null;
        }
        
        return new ChartSetInfo { DateAdded = DateTimeOffset.UtcNow };
    }
    
    protected override void Populate(ChartSetInfo model, ArchiveReader? archive, Realm realm, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (archive != null)
        {
            var chartFiles = archive.FileNames.Where(f => f.EndsWith(kumi_chart, StringComparison.OrdinalIgnoreCase));

            try
            {
                foreach (string chartFile in chartFiles)
                {
                    var imported = importChart(model, chartFile);
                    if (imported != null)
                        model.Charts.Add(imported);
                }
            } catch (TaskCanceledException)
            {
            } catch (Exception e)
            {
                Logger.Error(e, $"Failed to populate chart set {model.ID}");
                throw;
            }
        }
    }

    protected override void PostImport(ChartSetInfo model, Realm realm)
    {
        ProcessChart?.Invoke(model);
    }

    private ChartInfo? importChart(ChartSetInfo set, string name)
    {
        // TODO
        return null;
    }
}
