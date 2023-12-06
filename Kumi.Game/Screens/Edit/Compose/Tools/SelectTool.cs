using Kumi.Game.Screens.Edit.Blueprints;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Screens.Edit.Compose.Tools;

public class SelectTool : NoteCompositionTool
{
    public SelectTool()
        : base("Select")
    {
    }

    public override Drawable? CreateIcon()
        => new SpriteIcon { Icon = FontAwesome.Solid.MousePointer };

    public override PlacementBlueprint? CreatePlacementBlueprint()
        => null;
}
