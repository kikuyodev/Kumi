using Kumi.Game.Models;

namespace Kumi.Game.Database;

public interface IHasFiles
{
    /// <summary>
    /// The list of files this model contains.
    /// </summary>
    IList<RealmNamedFileUsage> Files { get; }

    /// <summary>
    /// The combined hash of all files in this model.
    /// </summary>
    public string Hash { get; set; }
}

