using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Graphics.Containers;

public partial class KumiScrollContainer : KumiScrollContainer<Drawable>
{
    public KumiScrollContainer(Direction scrollDirection = Direction.Vertical)
        : base(scrollDirection)
    {
    }
}

public partial class KumiScrollContainer<T> : ScrollContainer<T>
    where T : Drawable
{
    public KumiScrollContainer(Direction scrollDirection = Direction.Vertical)
        : base(scrollDirection)
    {
        ScrollbarOverlapsContent = false;
    }
    
    protected override ScrollbarContainer CreateScrollbar(Direction direction)
        => new KumiScrollbarContainer(direction);
    
    private partial class KumiScrollbarContainer : ScrollbarContainer
    {
        private const float scroll_bar_width = 4;
        
        protected override float MinimumDimSize => scroll_bar_width * 3;

        public KumiScrollbarContainer(Direction direction)
            : base(direction)
        {
            Blending = BlendingParameters.Additive;
            Masking = true;
            CornerRadius = 5;

            Size = new Vector2(scroll_bar_width);

            const float margin = 4;
            
            Margin = new MarginPadding
            {
                Left = direction == Direction.Vertical ? margin : 0,
                Right = direction == Direction.Vertical ? margin : 0,
                Top = direction == Direction.Horizontal ? margin : 0,
                Bottom = direction == Direction.Horizontal ? margin : 0,
            };

            Child = new Box
            {
                RelativeSizeAxes = Axes.Both
            };
        }

        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
            this.ResizeTo(new Vector2(scroll_bar_width)
            {
                [(int) ScrollDirection] = val
            }, duration, easing);
        }
    }
}
