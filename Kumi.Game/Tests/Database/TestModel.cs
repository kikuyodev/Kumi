using Kumi.Game.Database;
using Realms;

namespace Kumi.Game.Tests.Database;

[MapTo("Test")]
internal class TestModel : RealmObject, IHasGuidPrimaryKey
{
    [PrimaryKey]
    public Guid ID { get; }
        
    public string? Name { get; set; }
        
    public TestModel()
    {
        ID = Guid.NewGuid();
    }
}
