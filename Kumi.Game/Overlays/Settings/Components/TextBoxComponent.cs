using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Overlays.Settings.Components;

public partial class TextBoxComponent : CompositeDrawable, IHasCurrentValue<string>
{
    public Bindable<string> Current { get; set; } = new Bindable<string>();

    private readonly KumiTextBox textBox;
    
    public TextBoxComponent()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;

        InternalChild = textBox = new KumiTextBox
        {
            Width = SettingOverlay.TEXT_BOX_WIDTH,
            RelativeSizeAxes = Axes.Y,
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopRight
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        textBox.Current.BindTo(Current);
    }
}
