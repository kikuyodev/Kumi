using Kumi.Game.Graphics.Backgrounds;
using Kumi.Game.Screens.Backgrounds;

namespace Kumi.Game.Screens.Edit;

public partial class EditorScreen : KumiScreen
{
    public override BackgroundScreen CreateBackground() => new ChartBackground();
    public override float DimAmount => 0.5f;
    public override float ParallaxAmount => 0.001f;
    
    
}
