using Realms;

namespace Kumi.Game.Database;

public interface IRealmBackedDefaultStore<TModel> 
    where TModel : class
{
    /// <summary>
    /// Safely gets an item from the database.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    TModel? Get(Func<TModel, bool> query);

    /// <summary>
    /// Writes an item to the database.
    /// </summary>
    /// <param name="query">The query to get the item.</param>
    /// <param name="action">The action to commit.</param>
    void Write(Func<TModel, bool> query, Action<TModel> action);
    
    /// <summary>
    /// Gets all of the items in the database.
    /// </summary>
    IEnumerable<TModel> GetAll();
    
    /// <summary>
    /// Returns the default values for this store.
    /// </summary>
    IEnumerable<TModel> GetDefaultValues();
    
    /// <summary>
    /// Registers the default values for this store.
    /// </summary>
    void RegisterDefaults();
    
    /// <summary>
    /// Compares two objects to see if they are the same.
    /// </summary>
    /// <param name="model">The first model</param>
    /// <param name="other">The second model.</param>
    bool Compare(TModel model, TModel other);

    /// <summary>
    /// Resets the objects belonging to this store to their default values.
    /// </summary>
    void Reset();
}
