using Kumi.Game.IO;
using Realms;

namespace Kumi.Game.Models;

public class RealmFile : RealmObject, IFileInfo
{
    public string Hash { get; set; } = string.Empty;
}
