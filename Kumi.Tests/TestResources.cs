using System.Reflection;
using Kumi.Game.Charts;
using Kumi.Game.Models;
using Kumi.Game.Tests;
using osu.Framework.Audio;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Framework.Utils;

namespace Kumi.Tests;

/// <summary>
/// Represents a collection of resources used for testing.
/// </summary>
public class TestResources
{
    private static Assembly assembly { get; } = typeof(TestResources).Assembly;
    private static List<KeyValuePair<string, string>> temporaryFiles { get; } = new List<KeyValuePair<string, string>>();

    /// <summary>
    /// Gets a temporary storage for testing.
    /// </summary>
    public static TemporaryNativeStorage GetTemporaryStorage() => new TemporaryNativeStorage("KumiTestResources");

    /// <summary>
    /// Gets a resource store for testing.
    /// </summary>
    public static DllResourceStore GetResourceStore() => new DllResourceStore(assembly);

    public static TextureStore GetTextureStore(IRenderer renderer, GameHost host)
        => new TextureStore(renderer, host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(GetResourceStore(), "Resources")));

    /// <summary>
    /// Opens a resource from the test resource store.
    /// </summary>
    /// <param name="relativePath">The relative path in the store.</param>
    public static Stream OpenResource(string relativePath)
    {
        var temporaryPath = getTempFile(getExtensionFromName(relativePath));

        using (var stream = GetResourceStore().GetStream(@$"Resources/{relativePath}"))
        using (var fileStream = File.OpenWrite(temporaryPath))
        {
            stream.CopyTo(fileStream);
        }

        return File.OpenRead(temporaryPath);
    }

    /// <summary>
    /// Opens a resource from the test resource store, then returns the path to the temporary file.
    /// </summary>
    /// <param name="relativePath">The relative path in the store.</param>
    public static string OpenResourcePath(string relativePath)
    {
        var temporaryPath = getTempFile(getExtensionFromName(relativePath));

        using (var stream = GetResourceStore().GetStream(@$"Resources/{relativePath}"))
        using (var fileStream = File.OpenWrite(temporaryPath))
        {
            stream.CopyTo(fileStream);
        }

        return temporaryPath;
    }

    public static Texture OpenTexture(IRenderer renderer, GameHost host, string relativePath) => GetTextureStore(renderer, host).Get(relativePath);

    public static Stream OpenTestChartStream() => OpenResource("Archives/MuryokuP - Sweet Sweet Cendrillion Drug (Author).kcs");
    public static string GetTemporaryPathForChart() => OpenResourcePath("Archives/MuryokuP - Sweet Sweet Cendrillion Drug (Author).kcs");

    public static Stream OpenWritableTemporaryFile(string name)
    {
        if (!name.Contains('.'))
            throw new ArgumentException("Name needs a file extension.", nameof(name));

        if (temporaryFiles.Any(x => x.Key == name))
            return File.OpenWrite(temporaryFiles.First(x => x.Key == name).Value);

        var temporaryPath = getTempFile(getExtensionFromName(name));
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

        var temporaryPath = getTempFile(getExtensionFromName(name));
        Stream stream = File.OpenRead(temporaryPath);

        temporaryFiles.Add(new KeyValuePair<string, string>(name, temporaryPath));
        return stream;
    }

    public static string GetTemporaryFilename(string extension)
        => Guid.NewGuid() + "." + extension;

    public static ChartSetInfo CreateChartSet(int difficulties = 10)
    {
        var metadata = new ChartMetadata
        {
            Artist = $"Test Artist {RNG.Next(0, 50)}",
            Title = $"Test Title {RNG.Next(0, 50)}",
            Creator = new RealmUser { Username = $"Test Author {RNG.Next(0, 50)}" }
        };

        var chartSetInfo = new ChartSetInfo();

        for (var i = 0; i < difficulties; i++)
            createChartInfo();

        void createChartInfo()
        {
            var chartInfo = new ChartInfo
            {
                DifficultyName = $"Test Difficulty {Guid.NewGuid().ToString()}",
                InitialScrollSpeed = 1.2f,
                Metadata = metadata!.DeepClone()
            };

            chartSetInfo!.Charts.Add(chartInfo);
            chartInfo.ChartSet = chartSetInfo;
        }

        return chartSetInfo;
    }

    public static TestWorkingChart CreateWorkingChart(AudioManager audio, TextureStore textures, Action<Chart> createChartData)
    {
        var setInfo = CreateChartSet(1);
        var chart = new Chart(setInfo.Charts.First());

        createChartData(chart);

        return new TestWorkingChart(chart, audio, textures);
    }

    public static void Cleanup()
    {
        foreach (var filePair in temporaryFiles)
            File.Delete(filePair.Value);

        temporaryFiles.Clear();
    }

    private static string getTempFile(string extension) => GetTemporaryStorage().GetFullPath(@$"{Guid.NewGuid()}.{extension}");

    private static string getExtensionFromName(string name) => name.Split('.').Last();
}
