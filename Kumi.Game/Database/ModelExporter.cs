using Kumi.Game.Extensions;
using Kumi.Game.IO;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;
using SharpCompress.Common;
using SharpCompress.Writers.Zip;

namespace Kumi.Game.Database;

public abstract class ModelExporter<TModel> : IModelExporter<TModel>
    where TModel : RealmObject, IHasFiles, IHasGuidPrimaryKey
{
    /// <summary>
    /// The extension that this importer can handle.
    /// </summary>
    protected abstract string Extension { get; }

    /// <summary>
    /// The user's data storage.
    /// </summary>
    protected readonly UserDataStorage Files;

    private readonly RealmAccess realm;

    protected ModelExporter(Storage storage, RealmAccess realm)
    {
        this.realm = realm;
        
        exportLocation = storage.GetStorageForDirectory("export");
        Files = new UserDataStorage(realm, storage);
    }

    private readonly Storage exportLocation;

    public async Task Export(TModel model)
    {
        if (!model.IsManaged)
            model = realm.Realm.Find<TModel>(model.ID)!;
        
        var endFile = $@"{model.GetModelDisplayString(realm.Realm)}";
        IEnumerable<string> existing = exportLocation.GetFiles(string.Empty, $@"{endFile}*{Extension}");

        if (existing.Any())
        {
            var existingCount = existing.Count();
            endFile += $" ({existingCount})";
        }

        try
        {
            await using (var stream = exportLocation.CreateFileSafely($@"{endFile}{Extension}"))
            {
                await ExportModelToStream(model, stream);
            }
        } catch
        {
            exportLocation.Delete($@"{endFile}*{Extension}");
        }
    }

    protected virtual Task ExportModelToStream(TModel model, Stream output)
    {
        using (var writer = new ZipWriter(output, new ZipWriterOptions(CompressionType.Deflate)))
        {
            foreach (var file in model.Files)
            {
                try
                {
                    var stream = Files.GetStreamFor(file.File);
                    writer.Write(file.FileName, stream, DateTime.Now);
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Failed to export {file.FileName}.");
                }
            }
        }

        return Task.CompletedTask;
    }
}
