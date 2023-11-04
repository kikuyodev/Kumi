using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Tests.Database;
using NUnit.Framework;

namespace Kumi.Tests.Charts;

public class ChartTest : RealmTest
{
    [Test]
    public void TestImportSimple()
    {
        Game.RunTestWithRealmAsync(async (realm, storage) =>
        {
            var importer = new ChartImporter(storage, realm);
            var chartSet = await importer.Import(new ImportTask(TestResources.OpenTestChartStream(), "test.kcs"));

            Assert.AreEqual(1, chartSet!.Charts.Count);
        });
    }

    [Test]
    public void TestExportSimple()
    {
        Game.RunTestWithRealmAsync(async (realm, storage) =>
        {
            var exporter = new ChartExporter(storage, realm);
            var importer = new ChartImporter(storage, realm);
            var chartSet = await importer.Import(new ImportTask(TestResources.OpenTestChartStream(), "test.kcs"));
            
            await exporter.Export(chartSet!);
            
            Assert.AreEqual(1, storage.GetFiles(string.Empty, "export/*.kcs").Count());
        });
    }
}
