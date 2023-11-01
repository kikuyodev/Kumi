using osu.Framework.Extensions;

namespace Kumi.Game.Utils;

public class ArchiveUtils
{
    private static readonly byte[][] pkHeaders =
    {
        new byte[] { 0x50, 0x4B, 0x03, 0x04 },
        new byte[] { 0x50, 0x4B, 0x05, 0x06 },
        new byte[] { 0x50, 0x4B, 0x07, 0x08 },
    };
    
    public static bool IsZipArchive(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        
        using var reader = new BinaryReader(stream);
        var header = reader.ReadBytes(4);
        
        // Check if the header matches any of the known PK headers
        return pkHeaders.Any(h => h.SequenceEqual(header) || h.SequenceEqual(header.Reverse().ToArray())); // Compare against the reversed header as well, due to endianness
    }
    
    public static bool IsZipArchive(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return IsZipArchive(new MemoryStream(stream.ReadAllBytesToArray()));
    }
}
