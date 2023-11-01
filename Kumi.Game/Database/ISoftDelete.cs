namespace Kumi.Game.Database;

public interface ISoftDelete
{
    /// <summary>
    /// Whether this object is pending deletion.
    /// </summary>
    public bool DeletePending { get; set; }
}
