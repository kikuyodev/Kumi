using Kumi.Game.Database;
using Kumi.Game.Tests.Database;
using NUnit.Framework;
using NUnit.Framework.Internal;
using osu.Framework.Testing;
using Realms;
using Realms.Schema;

namespace Kumi.Tests.Database;

[TestFixture]
public class RealmBackedDefaultStoreTest : RealmTest
{
    
    [Test]
    public void TestRegisterDefaults()
    {
        Game.RunTestWithRealm((realm, _) =>
        {
            var store = new TestDefaultStore(realm);
            store!.RegisterDefaults();
        
            Assert.AreEqual(2, store.GetAll().Count());
            Assert.AreEqual("Test", store.GetAll().First().Name);
            Assert.AreEqual("Test2", store.GetAll().Last().Name);
        });
    }

    [Test]
    public void TestWriteValue()
    {
        Game.RunTestWithRealm((realm, _) =>
        {
            var store = new TestDefaultStore(realm);
            store!.RegisterDefaults();
        
            Assert.AreEqual(2, store.GetAll().Count());
            Assert.AreEqual("Test", store.Get(t => t.Name == "Test")!.Name);
            Assert.AreEqual("Test2", store.GetAll().Last().Name);
            
            store.Write(m => m.Name == "Test", m => m.Name = "Test3");
            
            Assert.AreEqual("Test3", store.Get(t => t.Name == "Test3")!.Name);
            Assert.AreEqual("Test2", store.Get(t => t.Name == "Test2")!.Name);
        });
    }

    [Test]
    public void TestReset()
    {
        Game.RunTestWithRealm((realm, _) =>
        {
            var store = new TestDefaultStore(realm);
            store!.RegisterDefaults();
        
            Assert.AreEqual(2, store.GetAll().Count());
            Assert.AreEqual("Test", store.Get(t => t.Name == "Test")!.Name);
            Assert.AreEqual("Test2", store.GetAll().Last().Name);
            
            store.Write(m => m.Name == "Test", m => m.Name = "Test3");
            
            Assert.AreEqual("Test3", store.Get(t => t.Name == "Test3")!.Name);
            Assert.AreEqual("Test2", store.Get(t => t.Name == "Test2")!.Name);
            
            store.Reset();
            
            Assert.AreEqual(2, store.GetAll().Count());
            Assert.AreEqual("Test", store.Get(t => t.Name == "Test")!.Name);
        });
    }

    internal class TestDefaultStore : RealmBackedDefaultStore<TestModel>
    {
        public TestDefaultStore(RealmAccess realm)
            : base(realm)
        {
        }
        
        public override ICollection<TestModel> GetDefaultValues()
        {
            return new List<TestModel>
            {
                new TestModel { Name = "Test" },
                new TestModel { Name = "Test2" }
            };
        }
    }
    
}
