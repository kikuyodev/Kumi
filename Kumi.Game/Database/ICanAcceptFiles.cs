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


namespace Kumi.Game.Database;

/// <summary>
/// A class that can accept files to be imported.
/// </summary>
public interface ICanAcceptFiles
{
    /// <summary>
    /// Import one or more items from the filesystem.
    /// </summary>
    /// <param name="paths">The files which should be imported.</param>
    Task Import(params string[] paths);

    /// <summary>
    /// Import one or more items from an archive.
    /// </summary>
    /// <param name="tasks">The tasks which will be imported.</param>
    Task Import(ImportTask[] tasks);
    
    /// <summary>
    /// An array of file extensions that this importer can handle.
    /// </summary>
    IEnumerable<string> HandledFileExtensions { get; }
}
