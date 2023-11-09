using JetBrains.Annotations;
using Kumi.Game.IO;
using Realms;

namespace Kumi.Game.Models;

public class RealmNamedFileUsage : EmbeddedObject, INamedFileUsage
{
    public RealmFile File { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public RealmNamedFileUsage(RealmFile file, string fileName)
    {
        File = file ?? throw new ArgumentNullException(nameof(file));
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
    }

    [UsedImplicitly]
    private RealmNamedFileUsage()
    {
    }

    IFileInfo INamedFileUsage.File => File;
}
