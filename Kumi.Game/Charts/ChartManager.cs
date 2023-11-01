using System.Diagnostics;
using System.Linq.Expressions;
using Kumi.Game.Database;
using Kumi.Game.IO;
using Kumi.Game.IO.Archives;
using Kumi.Game.Models;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace Kumi.Game.Charts;

public class ChartManager : ModelManager<ChartSetInfo>, IModelImporter<ChartSetInfo>, IWorkingChartCache
{
    public ITrackStore ChartTrackStore { get; }

    private readonly ChartImporter chartImporter;
    private readonly WorkingChartCache workingChartCache;

    public Action<ChartSetInfo>? ProcessChart { private get; set; }

    public ChartManager(Storage storage, RealmAccess realm, AudioManager audioManager, IResourceStore<byte[]> gameResources, GameHost? host = null, WorkingChart? defaultChart = null)
        : base(storage, realm)
    {
        var userResources = new UserDataStorage(realm, storage).Store;

        ChartTrackStore = audioManager.GetTrackStore(userResources);

        chartImporter = CreateChartImporter(storage, realm);
        chartImporter.ProcessChart = args => ProcessChart?.Invoke(args);

        workingChartCache = CreateWorkingChartCache(audioManager, gameResources, userResources, defaultChart, host);
    }

    protected virtual WorkingChartCache CreateWorkingChartCache(AudioManager audioManager, IResourceStore<byte[]> resources, IResourceStore<byte[]> storage,
        WorkingChart? defaultChart, GameHost? host)
        => new WorkingChartCache(ChartTrackStore, audioManager, resources, storage, defaultChart, host);

    protected virtual ChartImporter CreateChartImporter(Storage storage, RealmAccess realm)
        => new ChartImporter(storage, realm);

    public WorkingChart CreateNew()
    {
        var metadata = new ChartMetadata
        {
            // TODO: Fetch the logged in user instead.
            Author = new RealmUser
            {
                Username = "Yourself!"
            }
        };

        var chartSet = new ChartSetInfo
        {
            DateAdded = DateTimeOffset.UtcNow,
            Charts =
            {
                new ChartInfo(metadata)
            }
        };

        foreach (var chart in chartSet.Charts)
            chart.ChartSet = chartSet;

        var imported = chartImporter.ImportModel(chartSet);

        if (imported == null)
            throw new InvalidOperationException("Failed to import new chart.");

        return GetWorkingChart(imported.Charts.First());
    }

    public void Hide(ChartInfo chartInfo)
    {
        // Realm.Run(r =>
        // {
        //     using (var transaction = r.BeginWrite())
        //     {
        //         if (!char)
        //     }
        // });
    }

    public void Restore(ChartInfo chartInfo)
    {
    }

    public void RestoreAll()
    {
    }

    public List<ChartSetInfo> GetAllUsableCharts()
    {
        return Realm.Run(r =>
        {
            r.Refresh();
            return r.All<ChartSetInfo>().Where(c => !c.DeletePending).Detach();
        });
    }

    public ChartSetInfo? QueryChartSet(Expression<Func<ChartSetInfo, bool>> query)
        => Realm.Run(r => r.All<ChartSetInfo>().FirstOrDefault(query));

    public ChartInfo? QueryChart(Expression<Func<ChartInfo, bool>> query)
        => Realm.Run(r => r.All<ChartInfo>().FirstOrDefault(query));

    public IWorkingChart? DefaultChart => workingChartCache.DefaultChart;

    public virtual void Save(ChartInfo chartInfo, IChart chartContent)
        => save(chartInfo, chartContent);

    public void Delete(Expression<Func<ChartSetInfo, bool>>? filter = null)
    {
        Realm.Run(r =>
        {
            var items = r.All<ChartSetInfo>().Where(s => !s.DeletePending);

            if (filter != null)
                items = items.Where(filter);

            Delete(items.ToList());
        });
    }

    public void UndeleteAll()
    {
        Realm.Run(r => Undelete(r.All<ChartSetInfo>().Where(s => s.DeletePending).ToList()));
    }

    private void save(ChartInfo chartInfo, IChart chartContent)
    {
        var set = chartInfo.ChartSet;
        Debug.Assert(set != null);

        chartContent.ChartInfo = chartInfo;
        
        // todo
    }

    #region ICanAcceptFiles Implementation

    public Task Import(params string[] paths) => chartImporter.Import(paths);
    public Task Import(ImportTask[] tasks) => chartImporter.Import(tasks);
    public Task<IEnumerable<ChartSetInfo>> ImportModels(ImportTask[] tasks) => chartImporter.ImportModels(tasks);

    public Task<ChartSetInfo?> Import(ImportTask task, CancellationToken cancellationToken = default)
        => chartImporter.Import(task, cancellationToken);
    
    public ChartSetInfo? ImportModel(ChartSetInfo item, ArchiveReader? archiveReader = null, CancellationToken cancellationToken = default)
        => chartImporter.ImportModel(item, archiveReader, cancellationToken);

    public IEnumerable<string> HandledFileExtensions => chartImporter.HandledFileExtensions;

    #endregion

    #region IWorkingChartCache implementation

    public WorkingChart GetWorkingChart(ChartInfo? chartInfo, bool refetch = false)
    {
        if (chartInfo != null)
        {
            if (refetch)
                workingChartCache.Invalidate(chartInfo);

            var missingFiles = chartInfo.ChartSet?.Files.Count == 0;

            if (refetch || chartInfo.IsManaged || missingFiles)
            {
                var id = chartInfo.ID;
                chartInfo = Realm.Run(r => r.Find<ChartInfo>(id)?.Detach()) ?? chartInfo;
            }
            
            Debug.Assert(chartInfo.IsManaged != true);
        }

        return workingChartCache.GetWorkingChart(chartInfo)!;
    }
    
    WorkingChart IWorkingChartCache.GetWorkingChart(ChartInfo? chartInfo) => GetWorkingChart(chartInfo);

    void IWorkingChartCache.Invalidate(ChartSetInfo chart) => workingChartCache.Invalidate(chart);
    void IWorkingChartCache.Invalidate(ChartInfo chart) => workingChartCache.Invalidate(chart);
    
    public event Action<WorkingChart>? OnInvalidated
    {
        add => workingChartCache.OnInvalidated += value;
        remove => workingChartCache.OnInvalidated -= value;
    }

    #endregion
}
