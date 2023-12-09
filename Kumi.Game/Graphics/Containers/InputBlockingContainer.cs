using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Graphics.Containers;

public partial class InputBlockingContainer : Container
{
    public override bool HandleNonPositionalInput => false;
    public override bool HandlePositionalInput => false;
}
