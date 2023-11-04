using Realms;

namespace Kumi.Game.Database;

/// <summary>
/// A class backed by realm that handles certain objects with assignable default values.
/// </summary>
/// <typeparam name="TModel">The model.</typeparam>
public abstract class RealmBackedDefaultStore<TModel> : IRealmBackedDefaultStore<TModel>
    where TModel : RealmObject, IHasGuidPrimaryKey
{
    public RealmBackedDefaultStore(RealmAccess realmAccss)
    {
        realm = realmAccss;
    }
    
    private readonly RealmAccess realm;
    
    public TModel? Get(Func<TModel, bool> query) => realm.Run(r => r.All<TModel>().First(query));
    
    public ICollection<TModel> GetAll() => realm.Run(r => r.All<TModel>().ToList());
    
    public void Write(Func<TModel, bool> query, Action<TModel> action)
    {
        realm.Write(r =>
        {
            var item = r.All<TModel>().First(query);

            if (item == null)
                return;
            
            action(item);
        });
    }
    
    public void RegisterDefaults()
    {
        realm.Write(r =>
        {
            var defaultValues = GetDefaultValues();
            var existing = r.All<TModel>().ToList();

            foreach (var item in defaultValues)
            {
                if (existing.Any(e => Compare(e, item)))
                {
                    continue;
                }

                r.Add(item);
            }
        });
    }
    public abstract ICollection<TModel> GetDefaultValues();
    
    public virtual bool Compare(TModel model, TModel other)
    {
        return model == other;
    }

    public void Reset()
    {
        realm.Write(r =>
        {
            foreach (var item in r.All<TModel>())
            {
                r.Remove(item);
            }
        });
        
        RegisterDefaults();
    }
}
