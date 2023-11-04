using Kumi.Game.Charts;
using Kumi.Game.Models;
using Kumi.Game.Tests;
using NUnit.Framework;
using osu.Framework.Extensions;

namespace Kumi.Tests.Database;

[TestFixture]
public abstract partial class RealmTest
{
    private RealmTestGame? game;

    protected RealmTestGame Game => game ??= new RealmTestGame();

    [TearDown]
    public void TeardownGame()
    {
        // Dispose any resources from previous tests
        game?.Dispose();
        game = null;
    }

    protected static ChartSetInfo CreateChartSet()
    {
        RealmFile createRealmFile() => new RealmFile { Hash = Guid.NewGuid().ToString().ComputeSHA2Hash() };

        var metadata = new ChartMetadata
        {
            Title = "Test Chart",
            Artist = "Test Artist",
        };

        var chartSet = new ChartSetInfo
        {
            Charts =
            {
                new ChartInfo(metadata) { DifficultyName = "Easy" },
                new ChartInfo(metadata) { DifficultyName = "Normal" },
                new ChartInfo(metadata) { DifficultyName = "Hard" },
                new ChartInfo(metadata) { DifficultyName = "Oni" },
            },
            Files =
            {
                new RealmNamedFileUsage(createRealmFile(), "test [easy].kch"),
                new RealmNamedFileUsage(createRealmFile(), "test [normal].kch"),
                new RealmNamedFileUsage(createRealmFile(), "test [hard].kch"),
                new RealmNamedFileUsage(createRealmFile(), "test [oni].kch"),
            }
        };

        foreach (var c in chartSet.Charts)
            c.ChartSet = chartSet;

        return chartSet;
    }
}
