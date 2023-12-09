using System.Linq.Expressions;
using Kumi.Game.Charts;
using Kumi.Game.Database;
using osu.Framework.Platform;

namespace Kumi.Game.Scoring;

public class ScoreManager : ModelManager<ScoreInfo>, IModelImporter<ScoreInfo>
{
    private readonly ScoreImporter scoreImporter;

    public ScoreManager(Storage storage, RealmAccess realm)
        : base(storage, realm)
    {
        scoreImporter = new ScoreImporter(storage, realm);
    }

    public ScoreInfo? Query(Expression<Func<ScoreInfo, bool>> query)
    {
        return Realm.Run(r => r.All<ScoreInfo>().FirstOrDefault(query)?.Detach());
    }

    public void Delete(Expression<Func<ScoreInfo, bool>>? filter = null)
    {
        Realm.Run(r =>
        {
            var items = r.All<ScoreInfo>()
               .Where(s => !s.DeletePending);
            
            if (filter != null)
                items = items.Where(filter);

            Delete(items.ToList());
        });
    }
    
    public void Delete(ChartInfo chart)
    {
        Realm.Run(r =>
        {
            var chartScore = r.Find<ChartInfo>(chart.ID)!.Scores.ToList();
            Delete(chartScore);
        });
    }

    #region IModelImporter implementation

    public Task<IEnumerable<ScoreInfo>> ImportModels(ImportTask[] tasks)
        => scoreImporter.ImportModels(tasks);

    public Task Import(params string[] paths)
        => scoreImporter.Import(paths);

    public Task Import(ImportTask[] tasks)
        => scoreImporter.Import(tasks);

    public ScoreInfo Import(ScoreInfo score)
        => scoreImporter.ImportModel(score);

    public IEnumerable<string> HandledFileExtensions => scoreImporter.HandledFileExtensions;

    #endregion
}
