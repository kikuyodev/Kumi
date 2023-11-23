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

public interface IModelManager<TModel>
    where TModel : class
{
    /// <summary>
    /// Deletes an item from the database.
    /// </summary>
    bool Delete(TModel model);

    /// <summary>
    /// Deletes a list of items from the database.
    /// </summary>
    void Delete(List<TModel> items);

    /// <summary>
    /// Restores an item that was previously deleted.
    /// </summary>
    bool Undelete(TModel model);

    /// <summary>
    /// Restores a list of items that were previously deleted.
    /// </summary>
    void Undelete(List<TModel> items);
}
