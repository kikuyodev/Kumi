using Kumi.Game.Charts.Formats;
using Kumi.Game.Database;
using Kumi.Game.Extensions;
using Kumi.Game.IO.Archives;
using osu.Framework.Extensions;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace Kumi.Game.Charts;

public class ChartImporter : RealmModelImporter<ChartSetInfo>
{
    public const string KUMI_ARCHIVE = ".kcs";
    public const string KUMI_CHART = ".kch";

    public override IEnumerable<string> HandledFileExtensions => new[] { KUMI_ARCHIVE };

    protected override string[] HashableFileTypes => new[] { KUMI_CHART };

    public Action<ChartSetInfo>? ProcessChart { private get; set; }

    public ChartImporter(Storage storage, RealmAccess realm)
        : base(storage, realm)
    {
    }

    protected override ChartSetInfo? CreateModel(ArchiveReader archive)
    {
        var chartsExists = archive.FileNames.FirstOrDefault(f => f.EndsWith(KUMI_CHART, StringComparison.OrdinalIgnoreCase));

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
            var chartFiles = archive.FileNames.Where(f => f.EndsWith(KUMI_CHART, StringComparison.OrdinalIgnoreCase));

            try
            {
                foreach (var chartFile in chartFiles)
                {
                    var imported = importChart(model, archive, realm, chartFile);

                    if (imported != null)
                    {
                        using var stream = archive.GetStream(chartFile)!;
                        imported.Hash = stream.ComputeSHA2Hash();
                        imported.File!.FileName = imported.GetModelDisplayString(realm) + KUMI_CHART;
                        model.Charts.Add(imported);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to populate chart set {model.ID}");
                throw;
            }
        }
    }

    protected override void PostImport(ChartSetInfo model, Realm realm)
    {
        foreach (var chart in model.Charts)
            chart.UpdateLocalScores(realm);
        
        ProcessChart?.Invoke(model);
    }

    private ChartInfo? importChart(ChartSetInfo set, ArchiveReader archive, Realm realm, string name)
    {
        using var stream = archive.GetStream(name)!;
        var decoder = new ChartDecoder();

        try
        {
            var chart = decoder.Decode(stream);
            var metadata = chart.Metadata.DeepClone();

            var chartInfo = new ChartInfo(metadata)
            {
                ChartVersion = chart.ChartInfo.ChartVersion,
                DifficultyName = chart.ChartInfo.DifficultyName,
                InitialScrollSpeed = chart.ChartInfo.InitialScrollSpeed,
                ChartSet = set
            };

            realm.Add(metadata);
            realm.Add(chartInfo);
            return chartInfo;
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to import chart {name} in set {set.ID}");
        }

        return null;
    }
}
