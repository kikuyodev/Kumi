using System.Buffers;
using Microsoft.Toolkit.HighPerformance;
using osu.Framework.Extensions;
using osu.Framework.IO.Stores;
using SharpCompress.Archives.Zip;
using SixLabors.ImageSharp.Memory;

namespace Kumi.Game.IO.Archives;

public class ZipArchiveReader : ArchiveReader
{
    private readonly Stream archiveStream;
    private readonly ZipArchive archive;

    public ZipArchiveReader(Stream archiveStream, string? name = null)
        : base(name!)
    {
        this.archiveStream = archiveStream;
        archive = ZipArchive.Open(archiveStream);
    }

    public override Stream? GetStream(string name)
    {
        var entry = archive.Entries.SingleOrDefault(e => e.Key == name);
        if (entry == null)
            return null;

        var owner = MemoryAllocator.Default.Allocate<byte>((int) entry.Size);

        using (var s = entry.OpenEntryStream())
        {
            s.ReadToFill(owner.Memory.Span);
        }

        return new MemoryOwnerMemoryStream(owner);
    }

    public override void Dispose()
    {
        archive.Dispose();
        archiveStream.Dispose();
    }

    public override IEnumerable<string> FileNames => archive.Entries.Select(e => e.Key).ExcludeSystemFileNames();

    private class MemoryOwnerMemoryStream : Stream
    {
        private readonly IMemoryOwner<byte> owner;
        private readonly Stream stream;

        public MemoryOwnerMemoryStream(IMemoryOwner<byte> owner)
        {
            this.owner = owner;

            stream = owner.Memory.AsStream();
        }

        protected override void Dispose(bool disposing)
        {
            owner.Dispose();
            base.Dispose(disposing);
        }

        public override void Flush() => stream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);
        public override void SetLength(long value) => stream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);
        public override bool CanRead => stream.CanRead;
        public override bool CanSeek => stream.CanSeek;
        public override bool CanWrite => stream.CanWrite;
        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }
    }
}
