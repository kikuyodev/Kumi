using osu.Framework.Extensions;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiTabControl<T> : TabControl<T>
{
    protected override TabItem<T> CreateTabItem(T value) => new KumiTabItem(value);
    protected override Dropdown<T> CreateDropdown() => null!;

    protected override TabFillFlowContainer CreateTabFlow() => new TabFillFlowContainer
    {
        Direction = FillDirection.Horizontal,
        RelativeSizeAxes = Axes.Both,
        Depth = -1
    };

    private readonly Circle strip;

    public KumiTabControl()
    {
        RelativeSizeAxes = Axes.Both;
        TabContainer.Spacing = Vector2.Zero;

        Current.BindValueChanged(_ => onNewValue());

        AddInternal(strip = new Circle
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Size = new Vector2(0, 3),
            Colour = Colours.BLUE_ACCENT_LIGHT,
            Alpha = 0,
            Y = 5,
            EdgeEffect = new EdgeEffectParameters
            {
                Roundness = 3,
                Type = EdgeEffectType.Glow,
                Colour = Colours.BLUE_ACCENT_LIGHT.Opacity(0.15f),
                Radius = 4
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Scheduler.AddDelayed(onNewValue, 25);
    }

    private void onNewValue()
    {
        var drawable = TabContainer.Children.FirstOrDefault(c => c.Value != null && c.Value.Equals(Current.Value));

        if (drawable is not KumiTabItem tabItem)
            return;

        strip.MoveToX((drawable.DrawPosition.X + tabItem.Text?.AnchorPosition.X ?? 0) - (tabItem.Text!.DrawSize.X / 2f), 200, Easing.OutQuint);
        strip.ResizeWidthTo(tabItem.Text?.DrawSize.X ?? drawable.DrawSize.X, 200, Easing.OutQuint);
        strip.FadeIn(200, Easing.OutQuint);
    }

    public partial class KumiTabItem : TabItem<T>
    {
        private const float fade_duration = 200;

        public SpriteText? Text;
        public SpriteText? TextBold;

        public KumiTabItem(T value)
            : base(value)
        {
            RelativeSizeAxes = Axes.Both;
            
            Children = CreateContent();
        }
        
        protected virtual LocalisableString GetText() => $"{Value.GetDescription()}";

        protected virtual Drawable[] CreateContent()
            => new Drawable[]
            {
                Text = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = GetText(),
                    Font = KumiFonts.GetFont(size: 10),
                    Colour = Colours.GRAY_6
                },
                TextBold = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = GetText(),
                    Font = KumiFonts.GetFont(weight: FontWeight.SemiBold, size: 10),
                    Colour = Colours.GRAY_C,
                    Alpha = 0
                },
            };

        protected override bool OnHover(HoverEvent e)
        {
            updateState();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e) => updateState();
        protected override void OnActivated() => updateState();
        protected override void OnDeactivated() => updateState();

        private void updateState()
        {
            Text.FadeColour(Active.Value || IsHovered ? Colours.GRAY_C : Colours.GRAY_6, fade_duration, Easing.OutQuint);
            Text.FadeTo(Active.Value ? 0 : 1, fade_duration, Easing.OutQuint);
            TextBold.FadeTo(Active.Value ? 1 : 0, fade_duration, Easing.OutQuint);
        }
    }
}
