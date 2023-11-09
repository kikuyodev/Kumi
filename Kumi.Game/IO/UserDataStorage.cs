using Kumi.Game.Database;
using Kumi.Game.Extensions;
using Kumi.Game.Models;
using osu.Framework.Extensions;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace Kumi.Game.IO;

/// <summary>
/// A storage for user data backed by the Realm database.
/// </summary>
public class UserDataStorage
{
    private readonly RealmAccess realmAccess;

    /// <summary>
    /// The underlying resource store that fetches the data from the storage.
    /// </summary>
    public readonly IResourceStore<byte[]> Store;

    /// <summary>
    /// The storage for the user data.
    /// </summary>
    public readonly Storage Storage;

    public UserDataStorage(RealmAccess realmAccess, Storage storage)
    {
        this.realmAccess = realmAccess;

        Storage = storage.GetStorageForDirectory("data");
        Store = new StorageBackedResourceStore(Storage);
    }

    /// <summary>
    /// Stores a file in the user data storage, returning the file itself.
    /// This function copies the data to the storage if it is not already there.
    /// </summary>
    public RealmFile Add(Stream data, Realm realm, bool addToRealm = true)
    {
        var hash = data.ComputeSHA2Hash();
        var existing = realm.Find<RealmFile>(hash);
        var file = existing ?? new RealmFile { Hash = hash };

        if (!checkExists(file))
            copyToStorage(file, data);

        if (addToRealm && !file.IsManaged)
            realm.Add(file);

        return file;
    }

    public Stream GetStreamFor(RealmNamedFileUsage namedFile) => GetStreamFor(namedFile.File);

    /// <summary>
    /// Gets an open stream for the given <see cref="RealmFile" />.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <exception cref="InvalidOperationException">Thrown when the file doesn't exist.</exception>
    public Stream GetStreamFor(RealmFile file)
    {
        if (!checkExists(file))
            throw new InvalidOperationException($"File {file.Hash} does not exist in the storage.");

        return Store.GetStream(file.GetStoragePath());
    }

    public string GetPathFor(RealmFile file)
    {
        if (!checkExists(file))
            throw new InvalidOperationException($"File {file.Hash} does not exist in the storage.");

        return file.GetStoragePath();
    }

    private void copyToStorage(IFileInfo file, Stream data)
    {
        data.Seek(0, SeekOrigin.Begin);

        using (var output = Storage.CreateFileSafely(file.GetStoragePath()))
        {
            data.CopyTo(output);
        }

        data.Seek(0, SeekOrigin.Begin);
    }

    private bool checkExists(IFileInfo file)
    {
        var path = file.GetStoragePath();

        if (!Storage.Exists(path))
            return false;

        using var stream = Storage.GetStream(path);
        return stream.ComputeSHA2Hash() == file.Hash;
    }

    public void Cleanup()
    {
        Logger.Log(@"Cleaning up user data storage...");

        var totalFiles = 0;
        var removedFiles = 0;

        realmAccess.Write(r =>
        {
            var files = r.All<RealmFile>().ToList();

            foreach (var file in files)
            {
                totalFiles++;

                if (file.BacklinksCount > 0)
                    // File is still being used in a model somewhere...
                    continue;

                try
                {
                    removedFiles++;
                    Storage.Delete(file.GetStoragePath());
                    r.Remove(file);
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Could not delete databased file {file.Hash}");
                }
            }
        });

        Logger.Log($"Finished realm file cleanup. {removedFiles} of {totalFiles} files were removed.");
    }
}
