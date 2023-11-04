using Kumi.Game.Charts;
using Kumi.Game.IO;
using osu.Framework.Development;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace Kumi.Game.Database;

public class RealmAccess : IDisposable
{
    /// <summary>
    /// Version history:
    /// 1    2023-10-28   Initial version.
    /// </summary>
    private const int schema_version = 1;

    public string FileName { get; private set; }

    private Realm? updateRealm;

    public Realm Realm
    {
        get
        {
            if (!ThreadSafety.IsUpdateThread)
                throw new InvalidOperationException("Realm can only be accessed from the update thread.");

            return updateRealm ??= getInstance();
        }
    }

    private readonly Storage storage;

    private RealmConfiguration config => new RealmConfiguration(storage.GetFullPath(FileName))
    {
        SchemaVersion = schema_version,
        MigrationCallback = onMigrate
    };

    public RealmAccess(Storage storage, string fileName = "kumi.realm")
    {
        this.storage = storage;
        FileName = fileName;
        
        cleanupPendingDeletions();
    }

    public T Run<T>(Func<Realm, T> action)
    {
        if (ThreadSafety.IsUpdateThread)
            return action(Realm);

        using var r = getInstance();
        return action(r);
    }

    public void Run(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
            action(Realm);
        else
        {
            using var r = getInstance();
            action(r);
        }
    }

    public T Write<T>(Func<Realm, T> action)
    {
        if (ThreadSafety.IsUpdateThread)
            return write(Realm, action);
        
        using var r = getInstance();
        return write(r, action);
    }
    
    public void Write(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
            write(Realm, action);
        else
        {
            using var r = getInstance();
            write(r, action);
        }
    }
    
    /// <summary>
    /// Subscribes to notifications for all objects of type <typeparamref name="TModel"/> that match the query.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="TModel">The model.</typeparam>
    public IDisposable Subscribe<TModel>(NotificationCallbackDelegate<TModel> action) 
        where TModel : RealmObject
        => Subscribe(_ => true, action);
    
    /// <summary>
    /// Subscribes to notifications for all objects of type <typeparamref name="TModel"/> that match the query.
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="TModel">The model.</typeparam>
    public IDisposable Subscribe<TModel>(Func<TModel, bool> query, NotificationCallbackDelegate<TModel> action)
        where TModel : RealmObject
    {
        if (ThreadSafety.IsUpdateThread)
            return Realm.All<TModel>().Where(query).AsQueryable().SubscribeForNotifications(action);

        using var r = getInstance();
        return r.All<TModel>().Where(query).AsQueryable().SubscribeForNotifications(action);
    }

    public bool Compact()
    {
        return Realm.Compact(config);
    }

    private void onMigrate(Migration migration, ulong oldSchemaVersion)
    {
        for (var i = oldSchemaVersion + 1; i <= schema_version; i++)
            migrateTo(migration, i);
    }

    private void migrateTo(Migration migration, ulong targetSchemaVersion)
    {
        switch (targetSchemaVersion)
        {
        }
    }

    private static T write<T>(Realm realm, Func<Realm, T> func)
    {
        Transaction? transaction = null;

        try
        {
            if (!realm.IsInTransaction)
                transaction = realm.BeginWrite();

            var result = func(realm);
            transaction?.Commit();
            return result;
        }
        finally
        {
            transaction?.Dispose();
        }
    }
    
    private static void write(Realm realm, Action<Realm> action)
    {
        Transaction? transaction = null;

        try
        {
            if (!realm.IsInTransaction)
                transaction = realm.BeginWrite();

            action(realm);
            transaction?.Commit();
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    private Realm getInstance()
        => Realm.GetInstance(config);

    private void cleanupPendingDeletions()
    {
        try
        {
            using (var realm = getInstance())
            using (var transaction = realm.BeginWrite())
            {
                var pendingDeletionCharts = realm.All<ChartSetInfo>().Where(c => c.DeletePending);

                foreach (var c in pendingDeletionCharts)
                    realm.Remove(c);
            
                transaction.Commit();
            }
        
            new UserDataStorage(this, storage).Cleanup();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to clean up unused files. This may be due to the database schema being too old.");
        }
    }

    public void Dispose()
    {
        updateRealm?.Dispose();
        GC.SuppressFinalize(this);
    }
}
