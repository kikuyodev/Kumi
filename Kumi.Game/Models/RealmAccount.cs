using Kumi.Game.Online.API.Accounts;
using Realms;

namespace Kumi.Game.Models;

public class RealmAccount : EmbeddedObject, IAccount, IEquatable<RealmAccount>
{
    public int Id { get; set; } = 1;

    public string Username { get; set; } = string.Empty;

    public bool Equals(RealmAccount? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id && Username == other.Username;
    }
}
