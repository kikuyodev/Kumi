// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the PPY-LICENCE file in the repository root for full licence text.
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
