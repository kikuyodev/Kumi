using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiCheckbox : Checkbox
{
    private readonly SpriteText label;
    
    private LocalisableString labelText;
    
    public LocalisableString LabelText
    {
        get => labelText;
        set
        {
            labelText = value;
            label.Text = value;
        }
    }
    
    public KumiCheckbox()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        
        Children = new Drawable[]
        {
            label = new SpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Font = KumiFonts.GetFont(size: 14),
                Margin = new MarginPadding
                {
                    Vertical = 4
                }
            },
            new DrawableCheckbox(this)
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight
            }
        };
    }

    private partial class DrawableCheckbox : Container, IHasCurrentValue<bool>
    {
        private readonly Box background;
        private readonly SpriteIcon icon;

        public Bindable<bool> Current { get; set; }

        public DrawableCheckbox(KumiCheckbox parent)
        {
            Current = new BindableBool
            {
                BindTarget = parent.Current
            };
            
            RelativeSizeAxes = Axes.Both;
            Masking = true;
            CornerRadius = 5;
            FillMode = FillMode.Fit;
            FillAspectRatio = 1;

            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("509495")
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.Check,
                    Size = new(16),
                    Alpha = 0,
                    AlwaysPresent = true
                }
            };
            
            Current.BindValueChanged(onUserChange, true);
        }

        private void onUserChange(ValueChangedEvent<bool> c)
        {
            if (c.NewValue)
            {
                background.FadeColour(Color4Extensions.FromHex("00BFFF"), 200, Easing.OutQuint);
                icon.FadeInFromZero(200, Easing.OutQuint);
            }
            else
            {
                background.FadeColour(Color4Extensions.FromHex("121212"), 200, Easing.OutQuint);
                icon.FadeOutFromOne(200, Easing.OutQuint);
            }
        }
    }
}
