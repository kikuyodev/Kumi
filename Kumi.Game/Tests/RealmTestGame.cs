using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Kumi.Game.Database;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace Kumi.Game.Tests;

public partial class RealmTestGame : osu.Framework.Game
{
    private GameHost GameHost => new CleanRunHeadlessGameHost(callingMethodName: "");

    private List<Action> scheduledTasks = new();
    private bool isRunning;

    private void scheduleTask(Action task)
    {
        if (LoadState != LoadState.Loaded)
            scheduledTasks.Add(task);
        else
            Scheduler.Add(task);
    }

    public void Add([InstantHandle] Action action)
    {
        scheduleTask(() =>
        {
            action();
            isRunning = false;
            Exit();
        });
    }

    public void Add([InstantHandle] Func<Task> action)
    {
        scheduleTask(async () =>
        {
            await action().ConfigureAwait(true);
            isRunning = false;
            Exit();
        });
    }

    public void RunTestWithRealm([InstantHandle] Action<RealmAccess, Storage> testAction, [CallerMemberName] string caller = "")
    {
        Add(() =>
        {
            var defaultStorage = (Storage)Dependencies.Get(typeof(Storage));
            var testStorage = defaultStorage.GetStorageForDirectory("test");

            using (var realm = new RealmAccess(testStorage, $"{Guid.NewGuid().ToString()}.realm"))
            {
                Logger.Log($"Running test using realm file {testStorage.GetFullPath(realm.FileName)}");
                testAction(realm, testStorage);

                Logger.Log($"Final database size: {GetFileSize(testStorage, realm)}");
                realm.Compact();
                Logger.Log($"Final database size after compact: {GetFileSize(testStorage, realm)}");
            }
        });

        if (!isRunning)
        {
            isRunning = true;
            GameHost.Run(this);
        }
    }

    public void RunTestWithRealmAsync(Func<RealmAccess, Storage, Task> testAction, [CallerMemberName] string caller = "")
    {
        Add(async () =>
        {
            var testStorage = (Storage)Dependencies.Get(typeof(Storage));

            using (var realm = new RealmAccess(testStorage, $"{Guid.NewGuid().ToString()}.realm"))
            {
                Logger.Log($"Running test using realm file {testStorage.GetFullPath(realm.FileName)}");
                await testAction(realm, testStorage);

                Logger.Log($"Final database size: {GetFileSize(testStorage, realm)}");
                realm.Compact();
                Logger.Log($"Final database size after compact: {GetFileSize(testStorage, realm)}");
            }
        });

        if (!isRunning)
        {
            isRunning = true;
            GameHost.Run(this);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (scheduledTasks.Count > 0)
        {
            foreach (var task in scheduledTasks)
                Scheduler.Add(task);
            scheduledTasks.Clear();
        }
    }

    private long GetFileSize(Storage testStorage, RealmAccess realm)
    {
        try
        {
            using (var stream = testStorage.GetStream(realm.FileName))
                return stream?.Length ?? 0;
        } catch
        {
            return 0;
        }
    }
}
