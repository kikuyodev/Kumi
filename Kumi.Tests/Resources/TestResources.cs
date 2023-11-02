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
    private static List<string> temporaryFiles { get; } = new();
    
    /// <summary>
    /// Gets a temporary storage for testing.
    /// </summary>
    public static TemporaryNativeStorage GetTemporaryStorage() => new TemporaryNativeStorage("TestResources");
    
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
        
        temporaryFiles.Add(temporaryPath);
        return File.OpenRead(temporaryPath);
    }
    
    public static void Cleanup()
    {
        foreach (string file in temporaryFiles)
            File.Delete(file);
        
        temporaryFiles.Clear();
    }

    private static string getTempFile(string extension) => GetTemporaryStorage().GetFullPath(@$"{Guid.NewGuid()}.{extension}");
    
    private static string getExtensionFromName(string name) => name.Split('.').Last();
}
