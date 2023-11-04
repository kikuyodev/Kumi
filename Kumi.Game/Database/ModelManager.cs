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

using System.Diagnostics;
using Kumi.Game.Extensions;
using Kumi.Game.IO;
using Kumi.Game.Models;
using NuGet.Packaging;
using osu.Framework.Platform;
using Realms;

namespace Kumi.Game.Database;

public class ModelManager<TModel> : IModelManager<TModel>, IModelFileManager<TModel, RealmNamedFileUsage>
    where TModel : RealmObject, IHasGuidPrimaryKey, IHasFiles, ISoftDelete
{
    protected readonly RealmAccess Realm;

    private readonly UserDataStorage userDataStorage;

    public ModelManager(Storage storage, RealmAccess realm)
    {
        Realm = realm;
        userDataStorage = new UserDataStorage(realm, storage);
    }

    public void DeleteFile(TModel item, RealmNamedFileUsage file)
        => performFileOperation(item, managed => DeleteFile(managed, managed.Files.First(f => f.FileName == file.FileName), managed.Realm!));

    public void ReplaceFile(TModel item, RealmNamedFileUsage file, Stream contents)
        => performFileOperation(item, managed => ReplaceFile(file, contents, managed.Realm!));
    
    public void AddFile(TModel item, Stream contents, string filename)
        => performFileOperation(item, managed => AddFile(managed, contents, filename, managed.Realm!));

    private void performFileOperation(TModel item, Action<TModel> operation)
    {
        if (!item.IsManaged)
        {
            Realm.Write(realm =>
            {
                var managed = realm.Find<TModel>(item.ID);
                Debug.Assert(managed != null);
                operation(managed);

                item.Files.Clear();
                item.Files.AddRange(managed.Files.Detach());
            });
        }
        else
            operation(item);
    }

    public void DeleteFile(TModel item, RealmNamedFileUsage file, Realm realm)
    {
        item.Files.Remove(file);
    }

    public void ReplaceFile(RealmNamedFileUsage file, Stream contents, Realm realm)
    {
        file.File = userDataStorage.Add(contents, realm);
    }

    public void AddFile(TModel item, Stream contents, string filename, Realm realm)
    {
        var existing = item.GetFile(filename);

        if (existing != null)
        {
            ReplaceFile(existing, contents, realm);
            return;
        }

        var file = userDataStorage.Add(contents, realm);
        var namedUsage = new RealmNamedFileUsage(file, filename);
        
        item.Files.Add(namedUsage);
    }

    public void Delete(List<TModel> items)
    {
        if (items.Count == 0) return;

        foreach (var item in items)
            Delete(item);
    }
    
    public bool Delete(TModel? item)
    {
        return Realm.Write(r =>
        {
            if (!item.IsManaged)
                item = r.Find<TModel>(item.ID);

            if (item?.DeletePending != false)
                return false;

            item.DeletePending = true;
            return true;
        });
    }

    public void Undelete(List<TModel> items)
    {
        if (items.Count == 0) return;

        foreach (var item in items)
            Undelete(item);
    }
    
    public bool Undelete(TModel? item)
    {
        return Realm.Write(r =>
        {
            if (!item.IsManaged)
                item = r.Find<TModel>(item.ID);

            if (item?.DeletePending != true)
                return false;

            item.DeletePending = false;
            return true;
        });
    }
}
