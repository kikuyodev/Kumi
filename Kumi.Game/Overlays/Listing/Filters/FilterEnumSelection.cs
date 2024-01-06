using Kumi.Game.Graphics;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Filters;

public partial class FilterEnumSelection<T> : TabControl<T>
    where T : struct, Enum
{
    public FilterEnumSelection()
    {
        Items = Enum.GetValues<T>();
        
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }
    
    protected override Dropdown<T> CreateDropdown() => null!;

    protected override TabFillFlowContainer CreateTabFlow() => new TabFillFlowContainer
    {
        Direction = FillDirection.Full,
        Spacing = new Vector2(4),
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Depth = -1,
        Masking = true
    };

    protected override TabItem<T> CreateTabItem(T value) => new FilterSelectionItem(value);
    
    public partial class FilterSelectionItem : TabItem<T>
    {
        private const float fade_duration = 200;

        public SpriteText? Text;
        public SpriteText? TextBold;

        public FilterSelectionItem(T value)
            : base(value)
        {
            AutoSizeAxes = Axes.Both;
            
            Children = CreateContent();
        }

        protected virtual Drawable[] CreateContent()
            => new Drawable[]
            {
                Text = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = Value.GetDescription(),
                    Font = KumiFonts.GetFont(size: 10),
                    Colour = Colours.GRAY_6
                },
                TextBold = new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = Value.GetDescription(),
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
