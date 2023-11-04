using Kumi.Game.Extensions;
using Kumi.Game.IO;
using Kumi.Game.Models;
using NUnit.Framework;
using osu.Framework.Extensions;

namespace Kumi.Tests.Database;

public class UserDataStorageTests : RealmTest
{
    [Test]
    public void ImportFile()
    {
        Game.RunTestWithRealm((realm, storage) =>
        {
            var files = new UserDataStorage(realm, storage);
            var data = new MemoryStream("kumi"u8.ToArray());

            realm.Realm.Write(() => files.Add(data, realm.Realm));
            
            Assert.True(files.Storage.Exists(@"e5/5d/e55dfe4c54a6524b8e2bfab953aaf21ff43b900faae2dbb49d1b0861d647c15c"));
            Assert.True(files.Storage.Exists(realm.Realm.All<RealmFile>().First().GetStoragePath()));
            
            var file = files.Storage.GetStream(@"e5/5d/e55dfe4c54a6524b8e2bfab953aaf21ff43b900faae2dbb49d1b0861d647c15c");
            Assert.NotNull(file);
            Assert.AreEqual("kumi"u8.ToArray(), file!.ReadAllBytesToArray());
        });
    }

    [Test]
    public void ImportFileTwice()
    {
        Game.RunTestWithRealm((realm, storage) =>
        {
            var files = new UserDataStorage(realm, storage);
            var data = new MemoryStream("kumi"u8.ToArray());

            realm.Realm.Write(() => files.Add(data, realm.Realm));
            realm.Realm.Write(() => files.Add(data, realm.Realm));

            Assert.AreEqual(1, realm.Realm.All<RealmFile>().Count());
        });
    }

    [Test]
    public void DoesntPurgeReferenced()
    {
        Game.RunTestWithRealm((realmAccess, storage) =>
        {
            var realm = realmAccess.Realm;
            var files = new UserDataStorage(realmAccess, storage);
            var data = new MemoryStream("kumi"u8.ToArray());
            var file = realm.Write(() => files.Add(data, realm));

            realm.Write(() =>
            {
                var set = CreateChartSet();
                set.Files.Add(new RealmNamedFileUsage(file, "resource"));
                realm.Add(set);
            });
            
            Assert.True(realm.All<RealmFile>().Any());
            Assert.True(files.Storage.Exists(file.GetStoragePath()));
            
            files.Cleanup();
            Assert.True(realm.All<RealmFile>().Any());
            Assert.True(file.IsValid);
            Assert.True(files.Storage.Exists(file.GetStoragePath()));
        });
    }

    [Test]
    public void DoesPurgeUnreferenced()
    {
        Game.RunTestWithRealm((realmAccess, storage) =>
        {
            var realm = realmAccess.Realm;
            var files = new UserDataStorage(realmAccess, storage);
            var data = new MemoryStream("kumi"u8.ToArray());
            var file = realm.Write(() => files.Add(data, realm));
            
            Assert.True(realm.All<RealmFile>().Any());
            Assert.True(files.Storage.Exists(file.GetStoragePath()));
            
            files.Cleanup();
            
            Assert.False(realm.All<RealmFile>().Any());
            Assert.False(file.IsValid);
            Assert.False(files.Storage.Exists(file.GetStoragePath()));
        });
    }
}
