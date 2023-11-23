/*
    Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.

     Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using Kumi.Game.IO.Archives;
using Kumi.Game.Utils;
using osu.Framework.Extensions;
using SharpCompress.Common;

namespace Kumi.Game.Database;

/// <summary>
/// An encapsulated import task to be imported to an <see cref="IModelImporter{TModel}" />
/// </summary>
public class ImportTask
{
    /// <summary>
    /// The path to the file (or filename in the case a stream is provided).
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// An optional stream which provides the file content.
    /// </summary>
    public Stream? Stream { get; set; }

    /// <summary>
    /// Constructs a new import task from a path (on a local filesystem).
    /// </summary>
    public ImportTask(string path)
    {
        Path = path;
    }

    /// <summary>
    /// Constructs a new import task from a stream. The provided stream will be disposed after reading.
    /// </summary>
    public ImportTask(Stream? stream, string filename)
    {
        Path = filename;
        Stream = stream;
    }

    public ArchiveReader GetReader()
    {
        if (Stream == null)
        {
            if (ArchiveUtils.IsZipArchive(Path))
                return new ZipArchiveReader(File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read), System.IO.Path.GetFileName(Path));
            if (Directory.Exists(Path))
                return new DirectoryArchiveReader(Path);
            if (File.Exists(Path))
                return new SingleFileArchiveReader(Path);

            throw new InvalidFormatException($"{Path} is not a valid archive.");
        }

        if (Stream is not MemoryStream memoryStream)
        {
            memoryStream = new MemoryStream(Stream!.ReadAllBytesToArray());
            Stream.Dispose();
        }

        if (ArchiveUtils.IsZipArchive(memoryStream))
            return new ZipArchiveReader(memoryStream, Path);

        return new MemoryStreamArchiveReader(memoryStream, Path);
    }

    public virtual void DeleteFile()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }

    public override string ToString() => System.IO.Path.GetFileName(Path);
}
