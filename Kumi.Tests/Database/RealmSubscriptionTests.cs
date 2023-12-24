using Kumi.Game.Tests.Database;
using NUnit.Framework;
using Realms;

namespace Kumi.Tests.Database;

[TestFixture]
public class RealmSubscriptionTests : RealmTest
{
    [Test]
    public void TestSubscriptionCollectionChanges()
    {
        var collectionChanges = 0;
        var propertyChanges = 0;
        
        ChangeSet? lastChanges = null;
        
        Game.RunTestWithRealm((realm, _) =>
        {
            var registration = realm.Subscribe(r => r.All<TestModel>(), onChanged);

            realm.Run(r => r.Refresh());

            realm.Write(r => r.Add(new TestModel { Name = "Subscription tests" }));
            realm.Run(r => r.Refresh());
            
            Assert.That(collectionChanges, Is.EqualTo(1));
            Assert.That(propertyChanges, Is.EqualTo(0));
            Assert.That(lastChanges?.InsertedIndices, Has.One.Items);
            Assert.That(lastChanges?.ModifiedIndices, Is.Empty);
            Assert.That(lastChanges?.NewModifiedIndices, Is.Empty);
            
            realm.Write(r => r.All<TestModel>().First().Name = "Subscription tests 2");
            realm.Run(r => r.Refresh());
            
            Assert.That(collectionChanges, Is.EqualTo(1));
            Assert.That(propertyChanges, Is.EqualTo(1));
            Assert.That(lastChanges?.InsertedIndices, Is.Empty);
            Assert.That(lastChanges?.ModifiedIndices, Has.One.Items);
            Assert.That(lastChanges?.NewModifiedIndices, Has.One.Items);
            
            registration.Dispose();
        });

        void onChanged(IRealmCollection<TestModel> sender, ChangeSet? changes)
        {
            lastChanges = changes;
            
            if (changes == null)
                return;
            
            // if it has any collection changes
            if (changes.InsertedIndices.Length > 0 || changes.DeletedIndices.Length > 0 || changes.Moves.Length > 0)
                Interlocked.Increment(ref collectionChanges);
            else
                Interlocked.Increment(ref propertyChanges);
        }
    }
}
