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

public interface IModelFileManager<in TModel, in TFileModel>
    where TModel : class
    where TFileModel : class
{
    /// <summary>
    /// Adds a new file. If the file already exists, it will be overwritten.
    /// </summary>
    /// <param name="model">The item to add the file to.</param>
    /// <param name="contents">The file contents.</param>
    /// <param name="fileName">The name of the file.</param>
    void AddFile(TModel model, Stream contents, string fileName);

    /// <summary>
    /// Deletes an existing file.
    /// </summary>
    /// <param name="model">The item to delete the file from.</param>
    /// <param name="file">The existing file to delete.</param>
    void DeleteFile(TModel model, TFileModel file);

    /// <summary>
    /// Replace an existing file with new contents.
    /// </summary>
    /// <param name="model">The item to replace the file with.</param>
    /// <param name="file">The existing file to be replaced.</param>
    /// <param name="contents">The new file contents.</param>
    void ReplaceFile(TModel model, TFileModel file, Stream contents);
}
