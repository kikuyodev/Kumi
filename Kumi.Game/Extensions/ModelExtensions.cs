using Kumi.Game.Database;
using Kumi.Game.IO;
using Kumi.Game.Models;

namespace Kumi.Game.Extensions;

public static class ModelExtensions
{
    public static string GetStoragePath(this IFileInfo file)
        => Path.Combine(file.Hash.Remove(2), file.Hash.Substring(2, 2), file.Hash);

    public static string? GetPathForFile(this IHasFiles model, string filename)
        => model.GetFile(filename)?.File.GetStoragePath();

    public static RealmNamedFileUsage? GetFile(this IHasFiles model, string filename)
        => model.Files.SingleOrDefault(f => string.Equals(f.FileName, filename, StringComparison.OrdinalIgnoreCase));
}
