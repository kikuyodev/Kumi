using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Kumi.Game.Database;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace Kumi.Game.Tests;

public partial class RealmTestGame : osu.Framework.Game
{
    private GameHost GameHost => new CleanRunHeadlessGameHost(callingMethodName: "");
    private bool IsRunning => Scheduler.HasPendingTasks;
    public RealmTestGame()
    {
    }

    public void Add([InstantHandle] Action action)
    {
        Scheduler.Add(() =>
        {
            action();
            Exit();
        });
    }

    public void Add([InstantHandle] Func<Task> action)
    {
        Scheduler.Add(async () =>
        {
            await action().ConfigureAwait(true);
            Exit();
        });
    }

    public void RunTestWithRealm([InstantHandle] Action<RealmAccess, Storage> testAction, [CallerMemberName] string caller = "")
    {
        Add(() =>
        {
            var defaultStorage = GameHost.Storage;
            var testStorage = defaultStorage.GetStorageForDirectory("test");

            using (var realm = new RealmAccess(testStorage, $"{Guid.NewGuid().ToString()}.realm"))
            {
                Logger.Log($"Running test using realm file {testStorage.GetFullPath(realm.FileName)}");
                testAction(realm, testStorage);

                realm.Dispose();

                Logger.Log($"Final database size: {GetFileSize(testStorage, realm)}");
                realm.Compact();
                Logger.Log($"Final database size after compact: {GetFileSize(testStorage, realm)}");
            }
        });
        
        if (!IsRunning)
        {
            GameHost.Run(this);
        }
    }

    public void RunTestWithRealmAsync(Func<RealmAccess, Storage, Task> testAction, [CallerMemberName] string caller = "")
    {
        Add(async () =>
        {
            var testStorage = GameHost.Storage;

            using (var realm = new RealmAccess(testStorage, $"{Guid.NewGuid().ToString()}.realm"))
            {
                Logger.Log($"Running test using realm file {testStorage.GetFullPath(realm.FileName)}");
                await testAction(realm, testStorage);

                realm.Dispose();

                Logger.Log($"Final database size: {GetFileSize(testStorage, realm)}");
                realm.Compact();
                Logger.Log($"Final database size after compact: {GetFileSize(testStorage, realm)}");
            }
        });

        if (!IsRunning)
        {
            GameHost.Run(this);
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
