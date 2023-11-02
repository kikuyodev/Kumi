using System.Reflection;
using osu.Framework.IO.Stores;
using osu.Framework.Testing;

namespace Kumi.Tests.Resources;

/// <summary>
/// Represents a collection of resources used for testing.
/// </summary>
public class TestResources
{
    private static Assembly assembly { get; } = typeof(TestResources).Assembly;
    private static List<KeyValuePair<string, string>> temporaryFiles { get; } = new();
    
    /// <summary>
    /// Gets a temporary storage for testing.
    /// </summary>
    public static TemporaryNativeStorage GetTemporaryStorage() => new TemporaryNativeStorage("KumiTestResources");
    
    /// <summary>
    /// Gets a resource store for testing.
    /// </summary>
    public static DllResourceStore GetResourceStore() => new DllResourceStore(assembly);
    
    /// <summary>
    /// Opens a resource from the test resource store.
    /// </summary>
    /// <param name="relativePath">The relative path in the store.</param>
    public static Stream OpenResource(string relativePath)
    {
        string temporaryPath = getTempFile(getExtensionFromName(relativePath));
        
        using (var stream = GetResourceStore().GetStream(@$"Resources/{relativePath}"))
        using (var fileStream = File.OpenWrite(temporaryPath))
                stream.CopyTo(fileStream);
        
        return File.OpenRead(temporaryPath);
    }

    public static Stream OpenWritableTemporaryFile(string name)
    {
        if (!name.Contains('.'))
            throw new ArgumentException("Name needs a file extension.", nameof(name));

        if (temporaryFiles.Any(x => x.Key == name))
            return File.OpenWrite(temporaryFiles.First(x => x.Key == name).Value);
        
        string temporaryPath = getTempFile(getExtensionFromName(name));
        Stream stream = File.OpenWrite(temporaryPath);
        
        temporaryFiles.Add(new KeyValuePair<string, string>(name, temporaryPath));
        return stream;
    }
    public static Stream OpenReadableTemporaryFile(string name)
    {
        if (!name.Contains('.'))
            throw new ArgumentException("Name needs a file extension.", nameof(name));

        if (temporaryFiles.Any(x => x.Key == name))
            return File.OpenRead(temporaryFiles.First(x => x.Key == name).Value);
        
        string temporaryPath = getTempFile(getExtensionFromName(name));
        Stream stream = File.OpenRead(temporaryPath);
        
        temporaryFiles.Add(new KeyValuePair<string, string>(name, temporaryPath));
        return stream;
    }

    public static string GetTemporaryFilename(string extension)
        => Guid.NewGuid() + "." + extension;
    
    public static void Cleanup()
    {
        foreach (var filePair in temporaryFiles)
            File.Delete(filePair.Value);
        
        temporaryFiles.Clear();
    }

    private static string getTempFile(string extension) => GetTemporaryStorage().GetFullPath(@$"{Guid.NewGuid()}.{extension}");
    
    private static string getExtensionFromName(string name) => name.Split('.').Last();
}
