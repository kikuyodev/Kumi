// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the PPY-LICENCE file in the repository root for full licence text.
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

