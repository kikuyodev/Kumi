namespace Kumi.Game.Online;

public abstract class PlayerActivity
{
    /// <summary>
    /// Provides the large image key for the current activity
    /// </summary>
    public virtual string GetLargeImage() => "kumi-logo";
    
    /// <summary>
    /// Provides a string with the current detail of what the player is doing, such as chart name.
    /// </summary>
    public abstract string GetDetails();
    
    /// <summary>
    /// Provides a string with the current state of what the player is doing.
    /// </summary>
    public abstract string GetState();
    
    public class Idle : PlayerActivity
    {
        public override string GetDetails() => "";
        public override string GetState() => "Idle";
    }
}
