using Kumi.Game.Screens.Edit.Blueprints;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public abstract class NoteCompositionTool
{
    public readonly string Name;

    protected NoteCompositionTool(string name)
    {
        Name = name;
    }
    
    public abstract PlacementBlueprint? CreatePlacementBlueprint();

    public virtual Drawable? CreateIcon() => null;

    public override string ToString() => Name;
}
