// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the PPY-LICENCE file in the repository root for full licence text.
namespace Kumi.Game.Database;

public interface ISoftDelete
{
    /// <summary>
    /// Whether this object is pending deletion.
    /// </summary>
    public bool DeletePending { get; set; }
}
